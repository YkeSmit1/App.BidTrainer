using App.BidTrainer.Views;
using Application = Android.App.Application;

namespace App.BidTrainer.Droid
{
    public class FileAccessHelper : BidTrainerPage.IFileAccessHelper
    {
        public async Task<string> GetDataPathAsync()
        {
            var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            using var assets = Application.Context.Assets;
            foreach (var file in assets.List("data"))
            {
                await using var writeStream = new FileStream(Path.Combine(docFolder, file), FileMode.Create, FileAccess.Write);
                await using var stream = assets.Open(Path.Combine("data", file));
                await stream.CopyToAsync(writeStream);
            }

            return docFolder;
        }
    }
}