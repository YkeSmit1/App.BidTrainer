using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.BidTrainer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaderboardPage : ContentPage
    {
        public LeaderboardPage(IEnumerable<Account> accounts)
        {
            InitializeComponent();
            ((LeaderboardViewModel)BindingContext).Accounts = new ObservableCollection<Account>(accounts);

        }
    }
}