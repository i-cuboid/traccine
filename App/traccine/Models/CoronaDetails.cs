using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace traccine.Models
{
   public class CoronaDetails : INotifyPropertyChanged
   {
        public string country { get; set; }
        public string flag { get; set; }
        public Uri pic { get; set; }
        public int cases { get; set; }
        public int deaths { get; set; }
        public int recovered { get; set; }
        public int active { get; set; }
        public countryInfo countryInfo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
