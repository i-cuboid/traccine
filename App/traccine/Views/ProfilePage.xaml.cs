using Newtonsoft.Json;
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
    public partial class ProfilePage : ContentPage
    {
        public ProfilePageViewModel VM { get; }
        public FirbaseDataBaseHelper firebaseHelper { get; set; }
        public ProfilePage()
        {
            firebaseHelper = FirbaseDataBaseHelper.GetInstance;
            InitializeComponent();
            BindingContext = VM = new ProfilePageViewModel();
         
        }

        public async void OnDiagnosedSwitchToogled(object sender, ToggledEventArgs e)
        {
            if (VM != null)
            {
               
                if (VM.User.IsInfected)
                {
                    await firebaseHelper.Updateisinfected(VM.User.Id, VM.User.IsInfected);
                    Settings.User = JsonConvert.SerializeObject(VM.User);
                    VM.ReportedSymptoms = false;
                    VM.showSymptomsquestions = false;
                    VM.socialDistance = false;
                }
                else
                {
                    await firebaseHelper.Updateisinfected(VM.User.Id, VM.User.IsInfected);
                    Settings.User = JsonConvert.SerializeObject(VM.User);
                    VM.ReportedSymptoms = false;
                    VM.showSymptomsquestions = true;
                    VM.socialDistance = false;
                }
            }
        }
    }
}