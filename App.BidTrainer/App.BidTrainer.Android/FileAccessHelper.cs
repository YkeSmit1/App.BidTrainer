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
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(FileAccessHelper))]
namespace App.BidTrainer.Droid
{
    public class FileAccessHelper : BidTrainerPage.IFileAccessHelper
    {
        public string GetDataPath()
        {
            var docFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            using (var assets = Forms.Context.Assets)
            {
                foreach (var file in assets.List("data"))
                {
                    using var writeStream = new FileStream(Path.Combine(docFolder, file), FileMode.OpenOrCreate, FileAccess.Write);
                    assets.Open(Path.Combine("data", file)).CopyTo(writeStream);
                }
            }
            return docFolder;
        }
    }
}