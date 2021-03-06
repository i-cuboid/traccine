﻿using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using traccine.Helpers;
using traccine.Models;
using traccine.ViewModels;
using traccine.Views;
using Xamarin.Forms;

namespace traccine
{
    public partial class Carousel : ContentPage, INotifyPropertyChanged
    {
        private List<Color> _backgroundColors = new List<Color>();

        
        public object ItemsSource { get; internal set; }
        public Boolean buttonHide { get; set; }
        public Carousel()
        {
            InitializeComponent();

            var model = new CarouselViewModel
            {
                Items = new List<CarouselItem>()
                {
                    // Just create some dummy data here for now.
                    new CarouselItem{ Type="", ImageSrc="iconfinder_wash_hands_regulary_5964550.png", Name = "Wash Hands",  Title = "Wash Hands", Margin="0,0,-10,200", Description="Clean hands with soap and water or alcohol-based hand rub", BackgroundColor= Color.FromHex("#791AE5"), StartColor=Color.FromHex("#f3463f"),  EndColor=Color.FromHex("#fece49") , show=false},
                    new CarouselItem{ Type="", ImageSrc="iconfinder_facial_mask_coronavirus_5964544.png", Name = "Cover Your Mouth", Margin="0,0,130,250", Title = "Use Tissue", Description="Cover nose and mouth when coughing and sneezing with tissue or flexed elbow",  BackgroundColor= Color.FromHex("#fab62a"), StartColor=Color.FromHex("#42a7ff"),  EndColor=Color.FromHex("#fab62a"),show=false},
                    new CarouselItem{ Type="", ImageSrc="iconfinder_avoid_touch_eyes_mouth_face_5964543.png", Name = "Avoid Animals", Title = "Avoid", Margin="0,0,130,250", Description="Avoid touching your face", BackgroundColor= Color.FromHex("#425cfc"), StartColor=Color.FromHex("#33ccf3"),  EndColor=Color.FromHex("#ccee44"),show=true}
                }
            };

            BindingContext = model;

            // Create out a list of background colors based on our items colors so we can do a gradient on scroll.
            for (int i = 0; i < model.Items.Count; i++)
            {
                var current = model.Items[i];
                var next = model.Items.Count > i + 1 ? model.Items[i + 1] : null;

                if (next != null)
                    _backgroundColors.AddRange(SetGradients(current.BackgroundColor, next.BackgroundColor, 375));
                else
                    _backgroundColors.Add(current.BackgroundColor);
            }
        }

        public void Handle_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var position = e.HorizontalOffset;

            // Set the background color of our page to the item in the color gradient
            // array, matching the current scrollindex.
            if (position > _backgroundColors.Count - 1)
                page.BackgroundColor = _backgroundColors.Last();
            else if (position < 0)
                page.BackgroundColor = _backgroundColors.First();
            else
                page.BackgroundColor = _backgroundColors[(int)position];
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Need to start somewhere...
            page.BackgroundColor = _backgroundColors.First();
        }

        // Create a list of all the colors in between our start and end color.
        public static IEnumerable<Color> SetGradients(Color start, Color end, int steps)
        {
            var colorList = new List<Color>();

            double aStep = ((end.A * 255) - (start.A * 255)) / steps;
            double rStep = ((end.R * 255) - (start.R * 255)) / steps;
            double gStep = ((end.G * 255) - (start.G * 255)) / steps;
            double bStep = ((end.B * 255) - (start.B * 255)) / steps;

            for (int i = 0; i < steps; i++)
            {
                var a = (start.A * 255) + (int)(aStep * i);
                var r = (start.R * 255) + (int)(rStep * i);
                var g = (start.G * 255) + (int)(gStep * i);
                var b = (start.B * 255) + (int)(bStep * i);

                colorList.Add(Color.FromRgba(r / 255, g / 255, b / 255, a / 255));
            }

            return colorList;
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {

        Application.Current.MainPage = new TermsAndConditions();

        }
    }
}