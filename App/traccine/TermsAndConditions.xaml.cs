using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using traccine.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace traccine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermsAndConditions : ContentPage
    {
        public TermsAndConditions()
        {
            InitializeComponent();
        }
        private async void OnButtonClicked(object sender, EventArgs e)
        {

            Application.Current.MainPage = new MainPage();

        }
    }
}