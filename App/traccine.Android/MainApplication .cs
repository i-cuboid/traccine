using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Plugin.FirebasePushNotification;
using traccine.Models;

namespace traccine.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //Set the default notification channel for your app when running Android Oreo
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                //Change for your default notification channel id here
                FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";

                //Change for your default notification channel name here
                FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
            }


            //If debug you should reset the token each time.
#if DEBUG
            FirebasePushNotificationManager.Initialize(this, true);
#else
              FirebasePushNotificationManager.Initialize(this,true);
#endif

            //Handle notification when app is closed here
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                String Notification = "";
                String Body = "";
                foreach (var data in p.Data)
                {

                    // await service.GetSites();

                    if(data.Key == "title")
                    {
                        Notification = data.Value.ToString();
                    }else if(data.Key == "body")
                    {
                        Body = data.Value.ToString();
                    }
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");

                }
                if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                {
                    // Notification channels are new in API 26 (and not a part of the
                    // support library). There is no need to create a notification 
                    // channel on older versions of Android.
                    return;
                }
                using (var notificationManager = NotificationManager.FromContext(ApplicationContext))
                {
                    var title = Notification;
                    var channelName = "FCM_PUSHNOTIFICATIONS";
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        NotificationChannel channel = null;
                        if (channel == null)
                        {
                            channel = new NotificationChannel(channelName, channelName, NotificationImportance.Low)
                            {
                                LockscreenVisibility = NotificationVisibility.Public
                            };
                            channel.SetShowBadge(true);
                            notificationManager.CreateNotificationChannel(channel);
                        }
                        channel.Dispose();
                    }
                    Notifications _notifcation = new Notifications();
                    _notifcation.Body = Body;
                    _notifcation.Title = title;
                    _notifcation.Time = DateTime.UtcNow;
                    App.Database.AddNotification(_notifcation);
                    // var bitMap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.notification_template_icon_bg);
                    var notificationBuilder = new NotificationCompat.Builder(this, channelName)
                                                                            .SetContentTitle(title)
                                                                            .SetContentText(Body)
                                                                            .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                                                                            .SetShowWhen(false)
                                                                            .SetChannelId(channelName);

                    var notification = notificationBuilder.Build();
                    notificationManager.Notify(0, notification);
                }

            };

        }

      
    }
}