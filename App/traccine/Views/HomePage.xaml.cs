using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using traccine.Helpers;
using traccine.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace traccine.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public IBluetoothConnector _BluetoothConnector;
        public HomePageViewModel VM { get; }
        public bool IsLiveInitiated { get; set; }
        public HomePage()
        {
            InitializeComponent();
            BindingContext = VM= new HomePageViewModel();
            _BluetoothConnector = DependencyService.Get<IBluetoothConnector>();           
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (VM != null)
            {
                CheckUserPermissions();
                VM.Setup();
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
        public async void CheckUserPermissions()
        {
            try
            {
                var locationstatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (locationstatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Need location", "App needs location", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                    locationstatus = results[Permission.Location];
                }

                if (locationstatus == PermissionStatus.Granted)
                {

                }
                else if (locationstatus != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }

                var Storagestatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (Storagestatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await DisplayAlert("Need Storage", "App needs Storage", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                    Storagestatus = results[Permission.Storage];
                }

                if (Storagestatus == PermissionStatus.Granted)
                {

                }
                else if (Storagestatus != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Storage Denied", "Can not continue, try again.", "OK");
                }

                if (!await _BluetoothConnector.GetBluetoothStatus())
                {
                    bool answer = await DisplayAlert("", "AmiSafe wants to turn on bluetooth", "Yes", "No");
                    if (answer)
                    {
                        await _BluetoothConnector.EnableBluetooth(true);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}