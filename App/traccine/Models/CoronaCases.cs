using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace traccine.Models
{
    public class CoronaCases : INotifyPropertyChanged
    {
        public int cases { get; set; }
        public int deaths { get; set; }
        public int recovered { get; set; }
        public int active { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
