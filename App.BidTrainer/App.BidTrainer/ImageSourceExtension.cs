using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.BidTrainer
{
    [ContentProperty("Source")]
    public class ImageSourceExtension : BindableObject, IMarkupExtension<ImageSource>
    {
        public ImageSource ImageSource { get; set; }
        public ImageSource ProvideValue(IServiceProvider serviceProvider)
        {
            return ImageSource;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<ImageSource>).ProvideValue(serviceProvider);
        }
    }
}
