﻿using App.BidTrainer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.BidTrainer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ((SettingsViewModel)BindingContext).Load();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            var settingsViewModel = (SettingsViewModel)BindingContext;
            if (Preferences.Get("Username", "") != settingsViewModel.Username)
            {
                var account = await DependencyService.Get<ICosmosDBHelper>().GetAccount(settingsViewModel.Username);
                if (account.Value.id != null)
                {
                    await DisplayAlert("Error", "Username already exists", "OK");
                    return;
                }
            }

            settingsViewModel.Save();
        }
    }
}