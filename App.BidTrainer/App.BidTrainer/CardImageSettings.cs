using System;

namespace App.BidTrainer
{
    public class CardImageSettings
    {
        public string CardImage { get; set; }
        public bool FirstCardIsAce { get; set; }
        public int TopClubs { get; set; }
        public int TopDiamonds { get; set; }
        public int TopHearts { get; set; }
        public int TopSpades { get; set; }
        public int XOffSet { get; set; }
        public int CardWidth { get; set; }
        public int CardHeight { get; set; }
        public int XCardPadding { get; set; }
        public int CardDistance { get; set; }

        private static readonly CardImageSettings DefaultCardImageSettings = new CardImageSettings
        {
            CardImage = "App.BidTrainer.Resources.cardfaces.png",
            FirstCardIsAce = true,
            TopClubs = 0,
            TopDiamonds = 294,
            TopHearts = 196,
            TopSpades = 98,
            XOffSet = 0,
            CardWidth = 73,
            CardHeight = 97,
            XCardPadding = 0,
            CardDistance = 20
        };

        private static readonly CardImageSettings BboCardImageSettings = new CardImageSettings
        {
            CardImage = "App.BidTrainer.Resources.cardfaces2.jpg",
            FirstCardIsAce = false,
            TopClubs = 138,
            TopDiamonds = 14,
            TopHearts = 76,
            TopSpades = 198,
            XOffSet = 14,
            CardWidth = 38,
            CardHeight = 48,
            XCardPadding = 5,
            CardDistance = 32
        };

        public static CardImageSettings GetCardImageSettings(string settings)
        {
            return settings switch
            {
                "default" => DefaultCardImageSettings,
                "bbo" => BboCardImageSettings,
                _ => throw new NotImplementedException(),
            };
        }

    }
}
