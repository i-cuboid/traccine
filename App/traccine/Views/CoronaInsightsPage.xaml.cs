using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using traccine.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace traccine.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoronaInsightsPage : ContentPage
    {
        public CoronaInsightsPage()
        {
            InitializeComponent();
            BindingContext = new CoronaInsightsPageViewModel();
        }
    }
}