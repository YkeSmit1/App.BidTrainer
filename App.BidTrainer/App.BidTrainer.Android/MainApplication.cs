using Android.App;
using Android.Runtime;
using App.BidTrainer.Droid;
using App.BidTrainer.Views;

namespace App.BidTrainer.Android
{
	[Application]
	public class MainApplication : MauiApplication
	{
		public MainApplication(IntPtr handle, JniHandleOwnership ownership)
			: base(handle, ownership)
		{
			DependencyService.Register<BidTrainerPage.IFileAccessHelper, FileAccessHelper>();
			DependencyService.Register<ICosmosDbHelper, CosmosDbHelper> ();
		}

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
