using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using traccine.Models;
using traccine.Service;
using Xamarin.Forms;
using traccine.Helpers;
using Android.Bluetooth;

namespace traccine.Droid
{
    [Service(Name = "com.traccine.PeriodicService", Permission = "android.permission.BIND_JOB_SERVICE", Exported = true)]
    public class PeriodicService : IntentService
    {
        private BleServer _bleServer;
        private BleScannerService bleScannerService;
        private CancellationTokenSource _cts;
        public bool isSetupDone { get; set; }
        public const string PRIMARY_NOTIF_CHANNEL = "exampleChannel";
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

       


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }


        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            if ((Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O))
            {
                var channel = new NotificationChannel(PRIMARY_NOTIF_CHANNEL, "AmiSafe Service Channel", NotificationImportance.Low)
                {
                    Description = "Foreground Service Channel"
                };

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
                var Locintent = new Intent(BaseContext, typeof(MainActivity));

                var pending = PendingIntent.GetActivity(BaseContext, 0, intent, PendingIntentFlags.CancelCurrent);
                var notification = new Notification.Builder(this, PRIMARY_NOTIF_CHANNEL)
                .SetContentTitle(GlobalSettings.AppName)
                .SetContentText("Stay Safe With Covid-19")
                .SetAutoCancel(false)
                .SetSmallIcon(Android.Resource.Drawable.IcNotificationOverlay)
                .SetContentIntent(pending)
                //.SetOngoing(false)                
                .Build();
                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }
            ConnectionMonitor();
           
            HandleLiveEvent();
            return StartCommandResult.Sticky;
        }

        private void ConnectionSetup()
        {
           var  bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (Settings.User != "" && bluetoothAdapter.IsEnabled)
            {
                var t = new Java.Lang.Thread(() => {
                    _cts = new CancellationTokenSource();

                    Task.Run(() =>
                    {
                        try
                        {
                            _bleServer = new BleServer(this.ApplicationContext);


                        }
                        catch (Android.Accounts.OperationCanceledException)
                        {
                        }
                        finally
                        {
                            if (_cts.IsCancellationRequested)
                            {

                            }
                        }

                    }, _cts.Token);
                }
           );
                t.Start();
                Task.Run(async () =>
                {
                    bleScannerService = BleScannerService.GetInstance;

                });
                isSetupDone = true;
            }
           
        }

        public async void ConnectionMonitor()
        {
            await Task.Run(async () =>
            {
                while (!isSetupDone)
                {
                    ConnectionSetup();

                    Thread.Sleep(1000); // milliseconds to a second

                }
            });
        }
        public void HandleLiveEvent()
        {
            MessagingCenter.Subscribe<BleScannerService, string>(this, "OnInfectedDetected", (sender, args) => {


                using (var notificationManager = NotificationManager.FromContext(ApplicationContext))
                {
                    var title = "AmiSafe";
                    var channelName = "RecordDetected";
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
                    var intent = new Intent(BaseContext, typeof(MainActivity));
                    Notifications _notifcation = new Notifications();
                    _notifcation.Body = args;
                    _notifcation.Title = title;
                    _notifcation.Time = DateTime.UtcNow;
                    App.Database.AddNotification(_notifcation);
                    var pending = PendingIntent.GetActivity(BaseContext, 0, intent, PendingIntentFlags.CancelCurrent);
                    // var bitMap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.notification_template_icon_bg);
                    var notificationBuilder = new NotificationCompat.Builder(this, channelName)
                                                                            .SetContentTitle(title)                                                                         
                                                                            .SetContentText(args)
                                                                            .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                                                                            .SetShowWhen(false)
                                                                            .SetContentIntent(pending)
                                                                            .SetChannelId(channelName);

                    var notification = notificationBuilder.Build();
                    notificationManager.Notify(0, notification);
                }


            });
        }
        protected override void OnHandleIntent(Intent intent)
        {
         

        }
    }
}