using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace traccine.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SyncLoading : PopupPage, INotifyPropertyChanged
    {
        public SyncLoading(string DynamicString)
        {
            InitializeComponent();
            LoaderText.Text = DynamicString;
            CloseWhenBackgroundIsClicked = false;
        }
        protected override bool OnBackButtonPressed()
        {
            return true; // Disable back button
        }
    }
}