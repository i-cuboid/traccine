using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using traccine.Models;
using traccine.Views;
using Xamarin.Forms;
using Notifications = traccine.Models.Notifications;

namespace traccine.ViewModels
{
    public class NotificationViewModel :INotifyPropertyChanged
    {
        public ObservableCollection<Notifications> Notifications { get; set; }
        public bool NoNotificationsFound { get; set; }
        public bool NotificationsFound { get; set; }
        public ICommand ClearNotificationCommand { get; set; }
        public NotificationViewModel()
        {
            Notifications = new ObservableCollection<Notifications>();
            ClearNotificationCommand = new Command(ClearNotifications);
            NoNotificationsFound = false;
            NotificationsFound = false;
            setNotifications();
        }

        private async  void setNotifications()
        {
            Notifications.Clear();
            var _notifications = await App.Database.GetNotifications();
            foreach (var _notification in _notifications)
            {
                _notification.Time = _notification.Time.ToLocalTime();
                Notifications.Add(_notification);
            }
            if(Notifications.Count == 0)
            {
                NoNotificationsFound = true;
                NotificationsFound = false;
            }
            else
            {
                NoNotificationsFound = false;
                   NotificationsFound = true;
            }
        }

        private async  void ClearNotifications(object obj)
        {
            var page1 = new SyncLoading("Deleting Notifications...");
            await PopupNavigation.Instance.PushAsync(page1);

            await App.Database.RemoveNotification();
            setNotifications();
            await PopupNavigation.Instance.PopAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
