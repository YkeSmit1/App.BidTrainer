using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using MvvmHelpers.Commands;
using Common;
using EngineWrapper;
using App.BidTrainer.ViewModels;

namespace App.BidTrainer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BidTrainerPage : ContentPage
    {
        public interface IFileAccessHelper
        {
            string GetDataPath();
        }

        private readonly string dataPath = DependencyService.Get<IFileAccessHelper>().GetDataPath();
        private readonly StartPage startPage = new StartPage();
        private readonly SettingsPage settingsPage = new SettingsPage();
        private bool isInitialized = false;

        // Bidding
        private readonly BidManager bidManager = new BidManager();
        private readonly Auction auction = new Auction();
        private readonly Pbn pbn = new Pbn();
        private bool isInHintMode = false;

        // Lesson
        private static int CurrentLesson
        {
            get => Preferences.Get(nameof(CurrentLesson), 2);
            set => Preferences.Set(nameof(CurrentLesson), value);
        }
        private static int CurrentBoardIndex
        {
            get => Preferences.Get(nameof(CurrentBoardIndex), 0);
            set => Preferences.Set(nameof(CurrentBoardIndex), value);
        }
        private Dictionary<Player, string> Deal => pbn.Boards[CurrentBoardIndex].Deal;
        private Player Dealer => pbn.Boards[CurrentBoardIndex].Dealer;
        private List<Lesson> lessons;
        private Lesson Lesson => lessons.Single(l => l.LessonNr == CurrentLesson);

        // Results
        private Result currentResult;
        private DateTime startTimeBoard;
        private Results results = new Results();

        // ViewModels
        private BiddingBoxViewModel BiddingBoxViewModel => (BiddingBoxViewModel)BiddingBoxView.BindingContext;
        private AuctionViewModel AuctionViewModel => (AuctionViewModel)AuctionView.BindingContext;
        private HandViewModel HandViewModelNorth => (HandViewModel)panelNorth.BindingContext;
        private HandViewModel HandViewModelSouth => (HandViewModel)panelSouth.BindingContext;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        public BidTrainerPage()
        {
            InitializeComponent();
            Task.Run(() => Start());
        }

        private async Task Start()
        {
            try
            {
                LogManager.Configuration = new XmlLoggingConfiguration("assets/nlog.config");
                Application.Current.ModalPopping += PopModel;
                var lessonsFileName = Path.Combine(dataPath, "lessons.json");
                lessons = JsonConvert.DeserializeObject<List<Lesson>>(File.ReadAllText(lessonsFileName));
                BiddingBoxViewModel.DoBid = new AsyncCommand<object>(ClickBiddingBoxButton, ButtonCanExecute);
                AuctionViewModel.Auction = auction;
                var resultsFileName = Path.Combine(dataPath, "results.json");
                if (File.Exists(resultsFileName))
                    results = JsonConvert.DeserializeObject<Results>(File.ReadAllText(resultsFileName));
                isInitialized = true;

                await StartLesson();
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.ToString(), "OK");
                logger.Error(e);
                throw;
            }
        }

        public async Task StartLesson()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(startPage);
        }

        private async void PopModel(object sender, ModalPoppingEventArgs e)
        {
            if (e.Modal == startPage)
            {
                pbn.Load(Path.Combine(dataPath, Lesson.PbnFile));
                if (CurrentBoardIndex == 0)
                    results.AllResults.Remove(Lesson.LessonNr);
                await StartNextBoard();
            }
            else if (e.Modal == settingsPage)
            {
                ((SettingsViewModel)settingsPage.BindingContext).Save();
            }

        }

        private async Task ClickBiddingBoxButton(object parameter)
        {
            var bid = (Bid)parameter;
            if (isInHintMode)
            {
                currentResult.UsedHint = true;
                await DisplayAlert("Information", bidManager.GetInformation(bid, auction.currentPosition), "OK");
            }
            else
            {
                var engineBid = bidManager.GetBid(auction, Deal[Player.South]);
                UpdateBidControls(engineBid);

                if (bid != engineBid)
                {
                    await DisplayAlert("Incorrect bid", $"The correct bid is {engineBid}. Description: {engineBid.description}.", "OK");
                    currentResult.AnsweredCorrectly = false;
                }

                await BidTillSouth();
            }
        }

        private bool ButtonCanExecute(object param)
        {
            var bid = (Bid)param;
            return auction.BidIsPossible(bid);
        }

        private void UpdateBidControls(Bid bid)
        {
            auction.AddBid(bid);
            AuctionViewModel.UpdateAuction(auction);
            BiddingBoxViewModel.DoBid.RaiseCanExecuteChanged();
        }

        private async Task StartNextBoard()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                panelNorth.IsVisible = false;
                BiddingBoxView.IsEnabled = true;
            });
            if (CurrentBoardIndex > pbn.Boards.Count - 1)
            {
                CurrentBoardIndex = 0;

                if (Lesson.LessonNr != lessons.Last().LessonNr)
                {
                    CurrentLesson++;
                    pbn.Load(Path.Combine(dataPath, Lesson.PbnFile));
                }
                else
                {
                    BiddingBoxView.IsEnabled = false;
                    await DisplayAlert("Info", "End of lessons", "OK");
                    CurrentLesson = 2;
                    ShowReport();
                    return;
                }
            }
            await StartBidding();
        }

        private async Task StartBidding()
        {
            ShowBothHands();
            auction.Clear(Dealer);
            AuctionViewModel.UpdateAuction(auction);
            BiddingBoxViewModel.DoBid.RaiseCanExecuteChanged();
            bidManager.Init();
            StatusLabel.Text = $"Username: {Preferences.Get("Username", "")}\nLesson: {Lesson.LessonNr}\nBoard: {CurrentBoardIndex + 1}";
            startTimeBoard = DateTime.Now;
            currentResult = new Result();
            await BidTillSouth();
        }

        private void ShowBothHands()
        {
            var alternateSuits = Preferences.Get("AlternateSuits", true);
            Task.Run(() => HandViewModelNorth.ShowHand(Deal[Player.North], alternateSuits));
            Task.Run(() => HandViewModelSouth.ShowHand(Deal[Player.South], alternateSuits));
        }

        private async Task BidTillSouth()
        {
            while (auction.currentPlayer != Player.South && !auction.IsEndOfBidding())
            {
                var bid = bidManager.GetBid(auction, Deal[auction.currentPlayer]);
                UpdateBidControls(bid);
            }

            if (auction.IsEndOfBidding())
            {
                BiddingBoxView.IsEnabled = false;
                panelNorth.IsVisible = true;
                currentResult.TimeElapsed = DateTime.Now - startTimeBoard;
                await DisplayAlert("Info", $"Hand is done. Contract:{auction.currentContract}", "OK");
                results.AddResult(Lesson.LessonNr, CurrentBoardIndex, currentResult);
                UploadResults();
                CurrentBoardIndex++;
                File.WriteAllText(Path.Combine(dataPath, "results.json"), JsonConvert.SerializeObject(results, Formatting.Indented));

                await StartNextBoard();
            }
        }

        private void UploadResults()
        {
            var username = Preferences.Get("Username", "");
            if (username != "")
            {
                var res = results.AllResults.Values.SelectMany(x => x.Results.Values);
                Task.Run(() => UpdateOrCreateAccount(username, res.Count(), res.Count(x => x.AnsweredCorrectly), res.Sum(x => x.TimeElapsed.Ticks)));
            }

            static async Task UpdateOrCreateAccount(string username, int boardPlayed, int correctBoards, long timeElapsed)
            {
                var account = new Account
                {
                    username = username,
                    numberOfBoardsPlayed = boardPlayed,
                    numberOfCorrectBoards = correctBoards,
                    timeElapsed = new TimeSpan(timeElapsed)
                };

                var cosmosDBHelper = DependencyService.Get<ICosmosDBHelper>();
                var user = await cosmosDBHelper.GetAccount(username);
                if (user == null)
                {
                    account.id = Guid.NewGuid().ToString();
                    await cosmosDBHelper.InsertAccount(account);
                }
                else
                {
                    account.id = user.Value.id;
                    await cosmosDBHelper.UpdateAccount(account);
                }
            }
        }

        private void ShowReport()
        {
            var resultsPage = new ResultsPage(results);
            Application.Current.MainPage.Navigation.PushAsync(resultsPage);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await StartLesson();
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            CurrentBoardIndex++;
            await StartNextBoard();
        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {
            ShowReport();
        }

        private async void Button_Clicked_4(object sender, EventArgs e)
        {
            var accounts = await DependencyService.Get<ICosmosDBHelper>().GetAllAccounts();
            var leaderboardPage = new LeaderboardPage(accounts.OrderByDescending(x => (double)x.numberOfCorrectBoards / x.numberOfBoardsPlayed));
            await Application.Current.MainPage.Navigation.PushAsync(leaderboardPage);
        }

        private async void Button_Clicked_5(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(settingsPage);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (isInitialized)
            {
                StatusLabel.Text = $"Username: {Preferences.Get("Username", "")}\nLesson: {Lesson.LessonNr}\nBoard: {CurrentBoardIndex + 1}";
                ShowBothHands();
            }
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            isInHintMode = e.Value;
            labelMode.Text = isInHintMode ? "Hint" : "Bid";
        }
    }
}