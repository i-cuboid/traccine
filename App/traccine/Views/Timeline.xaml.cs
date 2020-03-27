using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using traccine.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace traccine.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Timeline : ContentPage
    {
        Button lastButton;
        public TimelineViewModel VM { get; }
        public bool IsLiveInitiated { get; set; }

        public Timeline()
        {
            InitializeComponent();
            BindingContext = VM =new TimelineViewModel();
            for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
            {
                Button btn = new Button()
                {
                    Text = $"{i}",                    
                    TextColor = Color.Black,
                    Padding = new Thickness(30, 10),
                    BackgroundColor = Color.Transparent,
                    CommandParameter= $"{i}"

                };
              
                btn.SetBinding(Button.CommandProperty, new Binding() { Path = "ViewCommand" });
                if (DateTime.Now.Day == i)
                {
                    VisualStateManager.GoToState(btn, "DaySelected");
                    lastButton = btn;
                   
                }
                //btn.SetBinding(Button.CommandParameterProperty, new Binding() { Path= $"{i}" } );
                btn.Clicked += (sender, EventArgs) =>
                {
                    if (lastButton != null)
                    {
                        VisualStateManager.GoToState(lastButton, "DayUnSelected");
                    }

                    VisualStateManager.GoToState((Button)sender, "DaySelected");

                    lastButton = (Button)sender;
                };
                DaysBlock.Children.Add(btn);
            }
          
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (VM != null)
            {
                SetupScroll();
                VM.HandleLiveEvent();
                IsLiveInitiated = true;
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (IsLiveInitiated)
            {
                VM.Unsubscribelive();
                IsLiveInitiated = false;
            }
        }
        private async void SetupScroll()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Update your children here, add more or remove.
                // todo

                if (Device.RuntimePlatform == Device.Android)
                {
                    await Task.Delay(10);
                    await StatusScrollView.ScrollToAsync(lastButton, ScrollToPosition.Center, true);

                }
                else
                {

                    await Task.Delay(10);
                    await StatusScrollView.ScrollToAsync(lastButton, ScrollToPosition.MakeVisible, true);
                }

            });

        }
    }
}