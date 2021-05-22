using MvvmHelpers;
using MvvmHelpers.Commands;
using MvvmHelpers.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App.BidTrainer.ViewModels
{
    public class StartViewModel : ObservableObject
    {
        public StartViewModel()
        {
            var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Lessons = JsonConvert.DeserializeObject<List<Lesson>>(File.ReadAllText(Path.Combine(dataPath, "lessons.json")));
        }
        public List<Lesson> Lessons { get; set; }
        public IAsyncCommand<int> StartLessonCommand { get; set; } = new AsyncCommand<int>(ChooseLesson);
        public IAsyncCommand ContinueWhereLeftOffCommand { get; set; } = new AsyncCommand(ContinueWhereLeftOff);

        private static async Task ChooseLesson(int lessonNr)
        {
            Preferences.Set("CurrentLesson", lessonNr);
            Preferences.Set("CurrentBoardIndex", 0);
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private static async Task ContinueWhereLeftOff()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
