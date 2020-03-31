using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using traccine.Models;
using traccine.Views;

namespace traccine.ViewModels
{
     public class CoronaInsightsPageViewModel
    {
        public ObservableCollection<CoronaDetails> CoronaDetails { get; set; }
        public CoronaInsightsPageViewModel()
        {
            CoronaDetails = new ObservableCollection<CoronaDetails>();
            setup();
        }

        private async void setup()
        {
            var page1 = new SyncLoading("Loading...");
            await PopupNavigation.Instance.PushAsync(page1);

           
          
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, "https://corona.lmao.ninja/countries?sort=cases"))
            {



                using (var httpClient = new HttpClient())
                {
                    var result = await httpClient.SendAsync(httpRequest);
                    CoronaDetails.Clear();
                    if (result.IsSuccessStatusCode)
                    {
                        var test = await result.Content.ReadAsStringAsync();
                        var Cases = JsonConvert.DeserializeObject<List<CoronaDetails>>(test);
                        foreach(var Case in Cases)
                        {
                            Case.pic = new Uri(Case.countryInfo.flag);
                            CoronaDetails.Add(Case);
                        }
                    }
                    else
                    {
                        // Use result.StatusCode to handle failure
                        // Your custom error handler here
                    }
                }
            }
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
