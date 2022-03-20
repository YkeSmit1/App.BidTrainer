using App.BidTrainer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.BidTrainer.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StartPage : ContentPage
	{
        public StartPage ()
		{
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await ((StartViewModel)BindingContext).LoadLessonsAsync();
        }
    }
}