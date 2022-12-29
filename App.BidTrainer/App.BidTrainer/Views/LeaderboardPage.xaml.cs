using System.Collections.ObjectModel;
using App.BidTrainer.ViewModels;

namespace App.BidTrainer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaderboardPage
    {
        public LeaderboardPage(IEnumerable<Account> accounts)
        {
            InitializeComponent();
            ((LeaderboardViewModel)BindingContext).Accounts = new ObservableCollection<Account>(accounts);

        }
    }
}