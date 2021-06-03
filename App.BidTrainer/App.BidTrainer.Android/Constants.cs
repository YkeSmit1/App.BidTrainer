using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace App.BidTrainer.Droid
{
    public static class Constants
    {
        internal static readonly string EndpointUri = "https://bidtrainer.documents.azure.com:443/";
        internal static readonly string PrimaryKey = "hIgAULhVaJKWI2Ro2RiMneMdwbvJoQGEW5Eu6XEByBaxuL02Jtrqwwfcp3yiuB0uFz0uSG4nLTVt63A1k4LuxQ==";
        internal static readonly string DatabaseName = "BidTrainer";
        internal static readonly string CollectionName = "Users";
    }
}