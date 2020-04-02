using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using traccine.Helpers;
using traccine.Models;
using traccine.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace traccine.ViewModels
{
    public class ProfilePageViewModel :INotifyPropertyChanged
    {
        public UserProfile User { get; set; }
        public FirbaseDataBaseHelper firebaseHelper { get; set; }
        public ICommand UpdateCommand { get; set; }

        public ICommand NotifyOthersCommand { get; set; }
        public int TotalActiveHours { get; set; }
        public int TotalScore { get; set; }
        public int TotalInteractions { get; set; }
        public Boolean Isconsultation { get; set; }
        public Boolean socialDistance { get; set; }
        public Boolean showSymptomsquestions { get; set; }
        public Boolean ReportedSymptoms { get; set; }
        public List<string> Questionlist { get; set; }
        public List<Symptomsoptions> Answers { get; set; }
        public ObservableCollection<SymptomsQuestions> Questions { get; set; }
        public FcmMessageHelper messageHelper;
        public ProfilePageViewModel()
        {
            Questionlist = new List<string>() { "Are you Suffering with Fever ?", "Are you experiencing any symptoms like 'Dry Cough', 'Sore throat' , 'Weakness' and 'Difficulty in Breathing' ?", "In the last 14 days, have you traveled internationally ?", "In the last 14 days, have you been in an area where COVID-19 is widespread?", "Do you live or work in a care facility?" };
            Answers = new List<Symptomsoptions>() { new Symptomsoptions { Text = "Yes" }, new Symptomsoptions { Text = "No" } };
            UpdateCommand = new Command(UpdateCommandAsync);
            NotifyOthersCommand = new Command(NotifyOthersAction);
            User = JsonConvert.DeserializeObject<UserProfile>(Settings.User);
            firebaseHelper = FirbaseDataBaseHelper.GetInstance;
            messageHelper = new FcmMessageHelper();
            TotalActiveHours = 0;
            TotalScore = 0;
            TotalInteractions = 0;
            ReportedSymptoms = false;
            socialDistance = false;
            showSymptomsquestions = true;
            Isconsultation = false;
            setup();
            Questions = new ObservableCollection<SymptomsQuestions>();
            PrepareQuestions();
        }

        private async void NotifyOthersAction(object obj)
        {
            var page1 = new SyncLoading("Notifiying to community...");
            await PopupNavigation.Instance.PushAsync(page1);
            await firebaseHelper.Updateisinfected(User.Id, User.IsInfected);
            if (User.IsInfected)
            {
              
                Settings.User = JsonConvert.SerializeObject(User);
                var users = await App.Database.GetTotalInteractedEmails();

                foreach (var user in users)
                {
                    var _interactedUser = await firebaseHelper.GetPerson(user);
                    if (_interactedUser.FcmToken != "")
                    {
                        await messageHelper.NotifyAsync(_interactedUser.FcmToken, "AmiSafe Alert", User.Name + " is Diagnosed +ve for COVID-19");
                    }
                }
            }
            await PopupNavigation.Instance.PopAsync();

        }

        private void PrepareQuestions()
        {
           
            foreach (var question in Questionlist)
            {
                SymptomsQuestions _question = new SymptomsQuestions();
                _question.Ques = question;
                _question.Answers = Answers;
                _question.Text = "";
                _question.SelectItem = new Symptomsoptions();
                Questions.Add(_question);
            }
        }

        private async void setup()
        {
            TotalActiveHours = await App.Database.GetTotalActiveHours();
            TotalInteractions = await App.Database.GetTotalInteractions();
        }

        private async void UpdateCommandAsync(object obj)
        {
            var page = new SyncLoading("Updating...");
            await PopupNavigation.Instance.PushAsync(page);

            Boolean IshavingFever = false;
            Boolean IshavingSymptoms = false;
            Boolean IshavingTravelhistory = false;
            Boolean IsVisitedtocoronaArea = false;
            Boolean IsheWorkingincarefacilty = false;

           

            foreach ( var question in Questions)
            {
                if(question.Ques == "Are you Suffering with Fever ?")
                {
                    if(question.SelectItem!=null)
                    {
                        if (question.SelectItem.Text == "Yes")
                        {
                            IshavingFever = true;
                        }
                    }
                      
                }else if (question.Ques == "Are you experiencing any symptoms like 'Dry Cough', 'Sore throat' , 'Weakness' and 'Difficulty in Breathing' ?")
                { 
                    if (question.SelectItem != null)
                    {
                        if (question.SelectItem.Text == "Yes")
                        {
                            IshavingSymptoms = true;
                        }
                    }
                }
                else if (question.Ques== "In the last 14 days, have you traveled internationally ?")
                {
                    if (question.SelectItem != null)
                    {
                        if (question.SelectItem.Text == "Yes")
                        {
                            IshavingTravelhistory = true;

                        }
                    }
                }
                else if (question.Ques ==  "In the last 14 days, have you been in an area where COVID-19 is widespread?")
                {
                    if (question.SelectItem != null)
                    {
                        if (question.SelectItem.Text == "Yes")
                        {
                            IsVisitedtocoronaArea = true;
                        }
                    }
                }
                else if (question.Ques ==  "Do you live or work in a care facility?")
                {
                    if (question.SelectItem != null)
                    {
                        if (question.SelectItem.Text == "Yes")
                        {
                            IsheWorkingincarefacilty = true;
                        }
                    }
                }
            }
            if(IshavingFever || IshavingSymptoms)
            {
                ReportedSymptoms = true;
                socialDistance = false;
                showSymptomsquestions = false;
                Isconsultation = false;

            }
            else if(IsheWorkingincarefacilty|| IsVisitedtocoronaArea || IshavingTravelhistory)
            {
                ReportedSymptoms = false;
                socialDistance = false;
                showSymptomsquestions = false;
                Isconsultation = true;
            }
            else
            {
                ReportedSymptoms = false;
                socialDistance = true;
                showSymptomsquestions = false;
                Isconsultation = false;
            }
            await PopupNavigation.Instance.PopAsync();
            var page1 = new SyncLoading("Notifiying to community...");
            await PopupNavigation.Instance.PushAsync(page1);
           
           
            if (ReportedSymptoms)
            {
                Settings.User = JsonConvert.SerializeObject(User);
                var users = await App.Database.GetTotalInteractedEmails();

                foreach (var user in users)
                {
                    var _interactedUser = await firebaseHelper.GetPerson(user);
                    if (_interactedUser.FcmToken != "")
                    {
                        await messageHelper.NotifyAsync(_interactedUser.FcmToken, "AmiSafe Alert ", User.Name + " is Reported COVID-19 Symptoms ");
                    }
                }
            }
           
            await PopupNavigation.Instance.PopAsync();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
