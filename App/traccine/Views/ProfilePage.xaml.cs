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
    public partial class ProfilePage : ContentPage
    {
        public ProfilePageViewModel VM { get; }
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = VM = new ProfilePageViewModel();
        }

        public void OnDiagnosedSwitchToogled(object sender, ToggledEventArgs e)
        {
            if (VM != null)
            {
                if (VM.User.IsInfected)
                {
                    VM.ReportedSymptoms = false;
                    VM.showSymptomsquestions = false;
                    VM.socialDistance = false;
                }
                else
                {
                    VM.ReportedSymptoms = false;
                    VM.showSymptomsquestions = true;
                    VM.socialDistance = false;
                }
            }
        }
    }
}