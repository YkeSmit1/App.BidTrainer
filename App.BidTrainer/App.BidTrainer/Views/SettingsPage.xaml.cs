using App.BidTrainer.ViewModels;
using System.Reflection;

namespace App.BidTrainer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            ImageDefault.Source = ImageSource.FromResource("App.BidTrainer.Resources.cardfaces.png", typeof(SettingsPage).GetTypeInfo().Assembly);
            ImageBbo.Source = ImageSource.FromResource("App.BidTrainer.Resources.cardfaces2.jpg", typeof(SettingsPage).GetTypeInfo().Assembly);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ((SettingsViewModel)BindingContext).Load();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            var settingsViewModel = (SettingsViewModel)BindingContext;
            if (Preferences.Get("Username", "") != settingsViewModel.Username)
            {
                var account = await DependencyService.Get<ICosmosDbHelper>().GetAccount(settingsViewModel.Username);
                if (account != null)
                {
                    await DisplayAlert("Error", "Username already exists", "OK");
                    return;
                }
            }

            settingsViewModel.Save();
        }
    }
}