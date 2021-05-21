using Common;
using MvvmHelpers;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace App.BidTrainer.ViewModels
{
    public class HandViewModel : BaseViewModel
    {
        public ObservableCollection<Card> Cards { get; set; } = new ObservableCollection<Card>();
        private readonly SKBitmap croppedBitmap;

        public HandViewModel()
        {
            croppedBitmap = LoadBitmapResource(GetType(), "App.BidTrainer.Resources.cardfaces.png");

            ShowHand("AQJ4,K32,843,QT9", true);
        }
        
        public static SKBitmap LoadBitmapResource(Type type, string resourceID)
        {
            var assembly = type.GetTypeInfo().Assembly;
            using var stream = assembly.GetManifestResourceStream(resourceID);
            return SKBitmap.Decode(stream);
        }

        public void ShowHand(string hand, bool alternateSuits)
        {
            Cards.Clear();
            var suitOrder = alternateSuits ?
                new List<Suit> { Suit.Spades, Suit.Hearts, Suit.Clubs, Suit.Diamonds } :
                new List<Suit> { Suit.Spades, Suit.Hearts, Suit.Diamonds, Suit.Clubs };
            var suits = hand.Split(',').Select((x, index) => (x, (Suit)(3 - index))).OrderBy(x => suitOrder.IndexOf(x.Item2));
            var index = 0;
            var width = 73;
            var height = 97;

            foreach (var suit in suits)
            {
                foreach (var card in suit.x)
                {
                    var cardwidth = (index == 12 ? width : 20);
                    var dest = new SKRect(0, 0, cardwidth, height);

                    var face = (int)Util.GetFaceFromDescription(card);
                    var topx = width * (int)face;
                    var topy = suit.Item2 switch
                    {
                        Suit.Clubs => 0,
                        Suit.Diamonds => 294,
                        Suit.Hearts => 196,
                        Suit.Spades => 98,
                        _ => throw new ArgumentException(nameof(suit)),
                    };

                    var bitmap = new SKBitmap(cardwidth, height);
                    var source = new SKRect(topx, topy, topx + cardwidth, topy + height);

                    // Copy 1/52 of the original into that bitmap
                    using (var canvas = new SKCanvas(bitmap))
                    {
                        canvas.DrawBitmap(croppedBitmap, source, dest);
                    }

                    var imgSource = (SKBitmapImageSource)bitmap;

                    Cards.Add(new Card { 
                        Index = new Rectangle(index * 20, 0, cardwidth, height), 
                        ImageSource = imgSource });
                    index++;
                }
            }
        }
    }
}
