using Newtonsoft.Json;
using Plugin.FirebasePushNotification;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using traccine.Helpers;
using traccine.Models;
using traccine.Views;
using Xamarin.Forms;

namespace traccine.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public UserProfile User { get; set; } = new UserProfile();
        public string Name
        {
            get => User.Name;
            set => User.Name = value;
        }

        public string Email
        {
            get => User.Email;
            set => User.Email = value;
        }

        public Uri Picture
        {
            get => User.Picture;
            set => User.Picture = value;
        }
        public string AppNmae { get; set; }

        public bool IsLoggedIn { get; set; }
        public FirbaseDataBaseHelper firebaseHelper { get; set; }

        public string Token { get; set; }
        private static readonly object Instancelock = new object();
        public ICommand LoginCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand OpenTermsCommand { get; set; }
        private readonly IGoogleClientManager _googleClientManager;
        public event PropertyChangedEventHandler PropertyChanged;
        public static MainPageViewModel _instance = null;
        public Boolean IsTermsAndConditionsAccepted { get; set; }
        public String AppName { get; set; }
        public string PowerdBy { get; set; }
        public static MainPageViewModel GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Instancelock)
                    {
                        if (_instance == null)
                        {
                            _instance = new MainPageViewModel();
                        }
                    }

                }
                return _instance;
            }
        }

        public MainPageViewModel()
        {
            LoginCommand = new Command(LoginAsync);
            AppNmae = GlobalSettings.AppName;
            LogoutCommand = new Command(Logout);
            OpenTermsCommand = new Command(GoToTerms);

            IsTermsAndConditionsAccepted = false;
              _googleClientManager = CrossGoogleClient.Current;
           firebaseHelper = FirbaseDataBaseHelper.GetInstance;
            AppName = GlobalSettings.AppName;
            PowerdBy = GlobalSettings.PowerdBy;
            IsLoggedIn = false;
           
           
        }
       public  void GoToTerms()
        {            
            PopupNavigation.Instance.PushAsync(new TermsAndConditions());
        }
        public async void LoginAsync()
        {
            var page = new SyncLoading("Loading...");
            await PopupNavigation.Instance.PushAsync(page);
            _googleClientManager.OnLogin += OnLoginCompleted;
            try
            {
                await _googleClientManager.LoginAsync();
            }
            catch (GoogleClientSignInNetworkErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientSignInCanceledErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientSignInInvalidAccountErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientSignInInternalErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientNotInitializedErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientBaseException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            PopupNavigation.Instance.PopAsync();
        }


        private async void OnLoginCompleted(object sender, GoogleClientResultEventArgs<GoogleUser> loginEventArgs)
        {
            if (loginEventArgs.Data != null)
            {

                GoogleUser googleUser = loginEventArgs.Data;
                User.Name = googleUser.Name;
                User.Email = googleUser.Email;
                User.Picture = googleUser.Picture;
                var GivenName = googleUser.GivenName;
                var FamilyName = googleUser.FamilyName;


                // Log the current User email
                Debug.WriteLine(User.Email);
                IsLoggedIn = true;
                
                var token = CrossGoogleClient.Current.ActiveToken;
                Token = token;
                var person = await firebaseHelper.GetPerson(User.Email);
                
                UserProfile user = new UserProfile();
                if (person == null)
                {
                 
                    user.Name = googleUser.Name;
                    user.Email = googleUser.Email;
                    user.Picture = googleUser.Picture;
                    user.IsTermsAndConditionsAccepted =IsTermsAndConditionsAccepted;
                    user.Id = Guid.NewGuid().ToString("N").Substring(0, 12);
                    await firebaseHelper.AddPerson(user.Id, user.Name, user.Email, "", user.Picture);
                    Settings.User = JsonConvert.SerializeObject(user);
                }
                else
                {
                    user.Name = person.Name;
                    user.Email = person.Email;
                    user.Picture = person.Picture;
                    user.Id = person.Id;
                    user.Picture = person.Picture;
                    user.PhoneNumber = person.PhoneNumber;
                    user.IsInfected = person.IsInfected;
                    await firebaseHelper.UpdateTermsandcondtions(user.Id, IsTermsAndConditionsAccepted);
                    Settings.User = JsonConvert.SerializeObject(user);
                   
                }
                await PopupNavigation.Instance.PopAsync();
                Application.Current.MainPage = new AppShell();
                Shell.Current.GoToAsync("//MainPage");


            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", loginEventArgs.Message, "OK");
            }

            _googleClientManager.OnLogin -= OnLoginCompleted;

        }

        public void Logout()
        {
            var token = _googleClientManager.ActiveToken;
            if (token != null)
            {
                _googleClientManager.OnLogout += OnLogoutCompleted;
                _googleClientManager.Logout();
            }
            else
            {
                Settings.User = "";
                Shell.Current.GoToAsync("//Login");
            }
          
        }

        private void OnLogoutCompleted(object sender, EventArgs loginEventArgs)
        {
            IsLoggedIn = false;
            User.Email = "Offline";
            _googleClientManager.OnLogout -= OnLogoutCompleted;
        }
    }
}
