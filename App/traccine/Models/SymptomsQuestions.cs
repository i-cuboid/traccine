using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace traccine.Models
{
    public class SymptomsQuestions : INotifyPropertyChanged
    {
        public string Ques { get; set; }
        public string Text { get; set; }
        public List<Symptomsoptions> Answers { get; set; }

        public Symptomsoptions SelectItem { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
