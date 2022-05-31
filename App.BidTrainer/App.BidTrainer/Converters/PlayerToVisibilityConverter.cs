using System;
using System.Globalization;
using Common;
using Xamarin.Forms;

namespace App.BidTrainer.Converters
{
    public class PlayerToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Player)value == Player.South;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
