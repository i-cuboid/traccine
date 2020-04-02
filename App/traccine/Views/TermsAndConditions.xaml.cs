using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using traccine.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace traccine.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermsAndConditions : PopupPage, INotifyPropertyChanged
    {
        public TermsAndConditions()
        {
            InitializeComponent();
            BindingContext = MainPageViewModel.GetInstance;
            CloseWhenBackgroundIsClicked = false;
        }

       

        public event PropertyChangedEventHandler PropertyChanged;
    }
}