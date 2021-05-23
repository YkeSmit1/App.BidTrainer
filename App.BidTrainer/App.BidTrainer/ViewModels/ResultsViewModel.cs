using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.BidTrainer.ViewModels
{
    public class ResultsViewModel : BaseViewModel
    {
        private Results results = new Results();
        public Results Results
        {
            get => results;
            set => SetProperty(ref results, value);
        }
    }
}
