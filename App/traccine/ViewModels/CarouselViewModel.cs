using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using traccine.Models;
using traccine.Views;
using Xamarin.Forms;

namespace traccine.ViewModels
{
    class CarouselViewModel : INotifyPropertyChanged
    {
        public ICommand CloseTermsCommand { get; set; }

        public List<CarouselItem> Items { get; set; } = new List<CarouselItem>();

        public event PropertyChangedEventHandler PropertyChanged;
        public CarouselViewModel()
        {
            CloseTermsCommand = new Command(GoToTerms); 
        }
        public void GoToTerms()
        {
            //PopupNavigation.Instance.PushAsync(new TermsAndConditions());
        }
    }
   
}
