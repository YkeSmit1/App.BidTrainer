using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace App.BidTrainer.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private string username;
        private bool alternateSuits;

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }
        public bool AlternateSuits 
        { 
            get => alternateSuits; 
            set => SetProperty(ref alternateSuits, value); 
        }

        public SettingsViewModel()
        {
            Load();
        }

        public void Load()
        {
            Username = Preferences.Get("Username", "");
            AlternateSuits = Preferences.Get("AlternateSuits", true);
        }

        public void Save()
        {
            Preferences.Set("Username", Username);
            Preferences.Set("AlternateSuits", AlternateSuits);
        }
    }
}
