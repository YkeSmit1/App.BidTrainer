using MvvmHelpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace App.BidTrainer.Views
{
    public class LeaderboardViewModel : ObservableObject
    {

        private ObservableCollection<Account> accounts;

        public ObservableCollection<Account> Accounts
        {
            get => accounts;
            set => SetProperty(ref accounts, value);
        }
    }
}