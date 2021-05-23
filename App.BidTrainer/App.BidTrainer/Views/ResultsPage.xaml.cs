using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App.BidTrainer.ViewModels;

namespace App.BidTrainer.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ResultsPage : ContentPage
	{
		public ResultsPage (Results results)
		{
			InitializeComponent ();
			((ResultsViewModel)BindingContext).Results = results;
		}

        private void Button_Clicked(object sender, EventArgs e)
        {
			Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}