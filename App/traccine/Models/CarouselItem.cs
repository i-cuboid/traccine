using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace traccine.Models
{
    public class CarouselItem : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public bool show { get; set; }
        public int Price { get; set; }
        public string Name { get; set; }
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }
        public Color BackgroundColor { get; set; }
        public string Type { get; set; }
        public string ImageSrc { get; set; }
        public string Description { get; set; }
        public string Margin { get; set; }

        private double _position;
        private double _scale;

        public CarouselItem()
        {
            Scale = 1;
        }

        public double Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged();
            }
        }

        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
