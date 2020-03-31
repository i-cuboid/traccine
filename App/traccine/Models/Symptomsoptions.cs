using System.ComponentModel;

namespace traccine.Models
{
    public class Symptomsoptions : INotifyPropertyChanged
    {
        public string Text { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}