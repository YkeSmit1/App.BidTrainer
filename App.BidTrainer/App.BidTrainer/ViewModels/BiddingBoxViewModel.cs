using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Common;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MvvmHelpers.Interfaces;

namespace App.BidTrainer.ViewModels
{
    public class BiddingBoxViewModel : BaseViewModel
    {
        public ObservableCollection<Bid> SuitBids { get; set; } = new ObservableCollection<Bid>();
        public ObservableCollection<Bid> NonSuitBids { get; set; } = new ObservableCollection<Bid>();

        public AsyncCommand<object> DoBid { get; set; }
        public bool IsEnabled { get; set; }

        public BiddingBoxViewModel()
        {
            SuitBids = new ObservableCollection<Bid>(Enum.GetValues(typeof(Suit)).Cast<Suit>()
                            .SelectMany(suit => Enumerable.Range(1, 7), (suit, level) => new { suit, level })
                            .Select(x => new Bid(x.level, x.suit)));
            NonSuitBids = new ObservableCollection<Bid> { Bid.PassBid, Bid.Dbl, Bid.Rdbl};
        }
    }
}
