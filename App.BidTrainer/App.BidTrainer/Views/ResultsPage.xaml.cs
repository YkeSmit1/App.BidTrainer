using App.BidTrainer.ViewModels;

namespace App.BidTrainer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultsPage
    {
        public ResultsPage(Results results)
        {
            InitializeComponent();
            ((ResultsViewModel)BindingContext).Results = results;
        }
    }
}