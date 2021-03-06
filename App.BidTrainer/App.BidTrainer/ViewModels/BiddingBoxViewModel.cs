using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using MvvmHelpers;
using MvvmHelpers.Commands;

namespace App.BidTrainer.ViewModels
{
    public class BiddingBoxViewModel : BaseViewModel
    {
        public ObservableCollection<Bid> SuitBids { get; set; }
        public ObservableCollection<Bid> NonSuitBids { get; set; }

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
