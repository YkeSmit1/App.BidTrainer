using App.BidTrainer.Views;
using MvvmHelpers;
using MvvmHelpers.Commands;
using MvvmHelpers.Interfaces;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App.BidTrainer.ViewModels
{
    public class StartViewModel : ObservableObject
    {
        public ObservableCollection<Lesson> Lessons { get; set; }
        public IAsyncCommand<int> StartLessonCommand { get; set; } = new AsyncCommand<int>(ChooseLesson);
        public IAsyncCommand ContinueWhereLeftOffCommand { get; set; } = new AsyncCommand(ContinueWhereLeftOff);

        public async Task LoadLessonsAsync()
        {
            var dataPath = await DependencyService.Get<BidTrainerPage.IFileAccessHelper>().GetDataPathAsync();
            Lessons = JsonConvert.DeserializeObject<ObservableCollection<Lesson>>(await File.ReadAllTextAsync(Path.Combine(dataPath, "lessons.json")));
            OnPropertyChanged(nameof(Lessons));
        }

        private static async Task ChooseLesson(int lessonNr)
        {
            Preferences.Set("CurrentLesson", lessonNr);
            Preferences.Set("CurrentBoardIndex", 0);
            await Shell.Current.GoToAsync("//BidTrainerPage");
        }

        private static async Task ContinueWhereLeftOff()
        {
            await Shell.Current.GoToAsync("//BidTrainerPage");
        }
    }
}
