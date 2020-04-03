using Newtonsoft.Json;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.Connectivity;
using Plugin.FirebasePushNotification;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using traccine.Helpers;
using traccine.Models;
using traccine.Service;
using traccine.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace traccine.ViewModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public string _Message { get; set; }
        public int Interactions { get; set; }
        public int ActiveHours { get; set; }
        public ICommand ShareCommand { get; set; }
        public ICommand GoToTimeLinePageCommand { get; set; }
        public ICommand GoToProfilePageCommand { get; set; }
        public ICommand NotificationsNavigateCommand { get; set; }
        public ICommand GoToInsightsPageCommand { get; set; }
        public int Cases { get; set; }
        public FirbaseDataBaseHelper firebaseHelper { get; set; }
        public int Score { get; set; }
        public String Username { get; set; }
        public bool NoInternet { get; set; }
        public string Message
        {
            get { return _Message; }
            set
            {
                if (_Message != value)
                {
                    _Message = value;
                    OnPropertyChanged("Message");
                }
            }
        }
        public HomePageViewModel()
        {
            NoInternet = false;
            ConnectionMonitor();
             Message = "";
            Cases = 0;
            Interactions = 0;
            Score = 0;
            ActiveHours = 0;
            Username = "";
            GoToInsightsPageCommand = new Command(GoToInsightsPage);
            ShareCommand = new Command(SahreCommand);
            GoToTimeLinePageCommand = new Command(GoToTimeLinePage);
            GoToProfilePageCommand = new Command(GoToProfilePage);
            NotificationsNavigateCommand = new Command(GoToNotificationsPage);
             firebaseHelper = FirbaseDataBaseHelper.GetInstance;
            if (Settings.User != "")
            {
                SetupFcm();
            }
        }
       
        public async void ConnectionMonitor()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        if (!NoInternet)
                        {
                            NoInternet = true;
                            var page1 = new SyncLoading("No Internet Connection");
                            await PopupNavigation.Instance.PushAsync(page1);
                        }
                       

                    }
                    else
                    {
                        if (NoInternet)
                        {
                            await PopupNavigation.Instance.PopAsync();
                            NoInternet = false;
                        }
                      //  return;
                    }

                    Thread.Sleep(1000); // milliseconds to a second

                }
            }); throw new NotImplementedException();
        }
        private async void GoToInsightsPage(object obj)
        {
            await Shell.Current.GoToAsync("CoronaInsightsPage");
        }

        private async void GoToNotificationsPage(object obj)
        {
            await Shell.Current.GoToAsync("NotificationsPage");
        }

        public async void SetupFcm()
        {
            CrossFirebasePushNotification.Current.OnTokenRefresh += async (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
                if (Settings.User != "")
                {
                    var existinguser = JsonConvert.DeserializeObject<UserProfile>(Settings.User);
                    var person = await firebaseHelper.GetPerson(existinguser.Email);
                    await firebaseHelper.UpdateFcmToken(existinguser.Id, p.Token);
                }

                CrossFirebasePushNotification.Current.UnsubscribeAll();
                CrossFirebasePushNotification.Current.Subscribe("AmiSafe_App");



                var topis = CrossFirebasePushNotification.Current.SubscribedTopics;

            };
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {

                System.Diagnostics.Debug.WriteLine("Received");

            };
            CrossFirebasePushNotification.Current.OnNotificationOpened += async (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Opened");
                await Shell.Current.GoToAsync("TimelinePage");
              

            };
        }
        private async void GoToProfilePage(object obj)
        {
            await Shell.Current.GoToAsync("ProfilePage");
        }

        private async void GoToTimeLinePage(object obj)
        {
           await Shell.Current.GoToAsync("TimelinePage");
        }

        public async void Setup()
        {
            Username = JsonConvert.DeserializeObject<UserProfile>(Settings.User).Name;
            ActiveHours= await App.Database.GetActiveHours();
            Interactions = await App.Database.GetInteraction();
            if (Interactions == 0)
            {
                Score = 100;
            }
            else
            {
                Score = 0;
            }
            if (DateTime.Now.Hour <= 12)
            {
                Message= "Good Morning";
            }
            else if (DateTime.Now.Hour <= 16)
            {
                Message = "Good Afternoon";
            }
            else if (DateTime.Now.Hour <= 20)
            {
                Message = "Good Evening";
           }
            else
            {
                Message = "Good Night";
           }
            if (CrossConnectivity.Current.IsConnected)
            {
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, "https://corona.lmao.ninja/all"))
                {



                    using (var httpClient = new HttpClient())
                    {
                        var result = await httpClient.SendAsync(httpRequest);

                        if (result.IsSuccessStatusCode)
                        {
                            var test = await result.Content.ReadAsStringAsync();
                            Cases = JsonConvert.DeserializeObject<CoronaCases>(test).cases;
                        }
                        else
                        {
                            // Use result.StatusCode to handle failure
                            // Your custom error handler here
                        }
                    }
                }
            }
        }
        public async void SahreCommand(object obj)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = GlobalSettings.AppUrl,
                Title = "Share AmiSafe",
                Text = "Hey, I'm using AmiSafe App, To break the Corona Virus Chain. Please install this app and join with me. "

            });
        }

        #region Live Feed
        public void HandleLiveEvent()
        {
            MessagingCenter.Subscribe<BleScannerService, string>(this, "RecordDetected", (sender, args) => {

                Setup();



            });
        }
        #endregion
        public void Unsubscribelive()
        {
            MessagingCenter.Unsubscribe<BleScannerService, string>(this, "RecordDetected");
        }
    }
}
