using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App.BidTrainer.Droid;
using App.BidTrainer.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(FileAccessHelper))]
namespace App.BidTrainer.Droid
{
    public class FileAccessHelper : BidTrainerPage.IFileAccessHelper
    {
        public async Task<string> GetDataPathAsync()
        {
            var docFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            using (var assets = Application.Context.Assets)
            {
                foreach (var file in assets.List("data"))
                {
                    using var writeStream = new FileStream(Path.Combine(docFolder, file), FileMode.Create, FileAccess.Write);
                    using var stream = assets.Open(Path.Combine("data", file));
                    await stream.CopyToAsync(writeStream);
                }
            }
            return docFolder;
        }
    }
}