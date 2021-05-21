using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Common;
using EngineWrapper;
using App.BidTrainer.ViewModels;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using MvvmHelpers.Commands;

namespace App.BidTrainer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BidTrainerPage : ContentPage
    {
        public interface IFileAccessHelper
        {
            string GetDataPath();
        }

        private string dataPath;
        // Bidding
        private readonly BidManager bidManager = new BidManager();
        private readonly Auction auction = new Auction();
        private readonly Pbn pbn = new Pbn();

        // Lesson
        //private static int CurrentBoardIndex => Settings1.Default.CurrentBoardIndex;
        public int CurrentBoardIndex { get; set; }
        private Dictionary<Player, string> Deal => pbn.Boards[CurrentBoardIndex].Deal;
        private Player Dealer => pbn.Boards[CurrentBoardIndex].Dealer;
        private Lesson lesson;
        private List<Lesson> lessons;

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
            Start();
        }

        private async Task Start()
        {
            LogManager.Configuration = new XmlLoggingConfiguration("assets/nlog.config");
            //MenuUseAlternateSuits.IsChecked = Settings1.Default.AlternateSuits;
            try
            {
                dataPath = DependencyService.Get<IFileAccessHelper>().GetDataPath();

                string lessonsFileName = Path.Combine(dataPath, "lessons.json");
                lessons = JsonConvert.DeserializeObject<List<Lesson>>(File.ReadAllText(lessonsFileName));

                BiddingBoxViewModel.DoBid = new AsyncCommand<object>(ClickBiddingBoxButtonAsync, ButtonCanExecute);
                AuctionViewModel.Auction = auction;
                string resultsFileName = Path.Combine(dataPath, "results.json");
                if (File.Exists(resultsFileName))
                    results = JsonConvert.DeserializeObject<Results>(File.ReadAllText(resultsFileName));

                await StartLessonAsync();
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.ToString(), "OK");
                logger.Error(e);
                throw;
            }
        }

        public async Task StartLessonAsync()
        {
            //var startPage = new StartPage();
            //startPage.ShowDialog();
            //lessons = startPage.Lessons;
            //if (!startPage.IsContinueWhereLeftOff)
            //    /*Settings1.Default.*/CurrentBoardIndex = 0;
            //lesson = startPage.Lesson;
            lesson = lessons[0];
            pbn.Load(Path.Combine(dataPath, lesson.PbnFile));
            //if (!startPage.IsContinueWhereLeftOff)
            //    results.AllResults.Remove(lesson.LessonNr);

            await StartNextBoardAsync();
        }

        private async Task ClickBiddingBoxButtonAsync(object parameter)
        {
            var bid = (Bid)parameter;
            //if (Cursor == Cursors.Help)
            //{
            //    currentResult.UsedHint = true;
            //    Cursor = Cursors.Arrow;
            //    MessageBox.Show(bidManager.GetInformation(bid, auction.currentPosition), "Information");
            //}
            //else
            {
                var engineBid = bidManager.GetBid(auction, Deal[Player.South]);
                UpdateBidControls(engineBid);

                if (bid != engineBid)
                {
                    await DisplayAlert("Incorrect bid", $"The correct bid is {engineBid}. Description: {engineBid.description}.", "OK");
                    currentResult.AnsweredCorrectly = false;
                }

                await BidTillSouthAsync();
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

        private async Task StartNextBoardAsync()
        {
            panelNorth.IsVisible = false;
            BiddingBoxView.IsEnabled = true;
            if (CurrentBoardIndex > pbn.Boards.Count - 1)
            {
                var newLessons = lessons.Where(x => x.LessonNr == lesson.LessonNr + 1);
                if (newLessons.Any())
                {
                    lesson = newLessons.Single();
                    pbn.Load(Path.Combine(dataPath, lesson.PbnFile));
                    /*Settings1.Default.*/
                    CurrentBoardIndex = 0;
                }
                else
                {
                    BiddingBoxView.IsEnabled = false;
                    await DisplayAlert("Info", "End of lessons", "OK");
                    //ShowReport();
                    return;
                }
            }
            await StartBiddingAsync();
        }

        private async Task StartBiddingAsync()
        {
            ShowBothHands();
            auction.Clear(Dealer);
            AuctionViewModel.UpdateAuction(auction);
            BiddingBoxViewModel.DoBid.RaiseCanExecuteChanged();
            bidManager.Init();
            StatusLabel.Text = $"Lesson: {lesson.LessonNr} Board: {CurrentBoardIndex + 1}";
            startTimeBoard = DateTime.Now;
            currentResult = new Result();
            await BidTillSouthAsync();
        }

        private void ShowBothHands()
        {
            HandViewModelNorth.ShowHand(Deal[Player.North], /*MenuUseAlternateSuits.IsChecked*/true);
            HandViewModelSouth.ShowHand(Deal[Player.South], /*MenuUseAlternateSuits.IsChecked*/true);
        }

        private async Task BidTillSouthAsync()
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
                results.AddResult(lesson.LessonNr, CurrentBoardIndex, currentResult);
                /*Settings1.Default.*/
                CurrentBoardIndex++;
                await StartNextBoardAsync();
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Start();
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            CurrentBoardIndex++;
            await StartNextBoardAsync();
        }
    }
}