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
    public partial class SponsorPage : ContentPage
    {
        public string Powerdby { set; get; }
        public SponsorPage()
        {
            InitializeComponent();
           
            BindingContext = new SponsorPageViewModel();
        }
        
        [Obsolete]
        private void iCuboidUrl(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(GlobalSettings.iCuboidUrl));
        }

        [Obsolete]
        private void DyocenseUrl(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(GlobalSettings.DyocenseUrl));
        }
        [Obsolete]
        private void CyclonerakeUlr(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(GlobalSettings.CyclonerakeUrl));
        }
    }
}