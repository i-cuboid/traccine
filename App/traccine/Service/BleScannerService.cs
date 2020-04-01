using Newtonsoft.Json;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using traccine.Helpers;
using traccine.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace traccine.Service
{
    public class BleScannerService
    {
        private static readonly object Instancelock = new object();
        public FirbaseDataBaseHelper firebaseHelper { get; set; }
        public Boolean NotifyUser { get; set; }
        public static BleScannerService _instance = null;
        public static BleScannerService GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Instancelock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BleScannerService();
                        }
                    }

                }
                return _instance;
            }

        }

        public void Reset()
        {
            lock (BleScannerService.Instancelock)
            {
                BleScannerService._instance = null;
            }
        }
        public BleScannerService()
        {
            RunTask();
          firebaseHelper = FirbaseDataBaseHelper.GetInstance;
        }
       public double getDistance(int rssi, int txPower)
        {
            /*
             * RSSI = TxPower - 10 * n * lg(d)
             * n = 2 (in free space)
             * 
             * d = 10 ^ ((TxPower - RSSI) / (10 * n))
             */

            return Math.Pow(10d, ((double)txPower - rssi) / (10 * 2));
        }
        private async void RunTask()
        {
            var ble = CrossBluetoothLE.Current;
         
            var adapter = CrossBluetoothLE.Current.Adapter;
           
            List<IDevice> deviceList = new List<IDevice>();
            adapter.DeviceDiscovered += (s, a) =>
            {

                deviceList.Add(a.Device);
            };
            while (true)
            {
                
               
                await adapter.StartScanningForDevicesAsync();
               
                foreach (var device in deviceList.ToList())
                {
                    var manufacturedata = device.AdvertisementRecords.Where(x => x.Type == AdvertisementRecordType.ManufacturerSpecificData).FirstOrDefault();
                    var manufactureinfo =Encoding.UTF8.GetString(manufacturedata.Data);
                    string my_String = Regex.Replace(manufactureinfo, @"[^0-9a-zA-Z]+", "");
                    if (my_String == "Ami")
                    {
                        try
                        {
                            Thread.Sleep(100);
                            await adapter.ConnectToDeviceAsync(device);
                        }
                        catch (DeviceConnectionException e)
                        {
                            continue;
                        }
                        var service = await device.GetServiceAsync(Guid.Parse("ffe0ecd2-3d16-4f8d-90de-e89e7fc396a5"));
                        if (service != null)
                        {

                            string data = "";
                           
                            var characteristic3 = await service.GetCharacteristicAsync(Guid.Parse("d8de624e-140f-4a22-8594-e2216b84a5f2"));
                            var bytes3 = await characteristic3.ReadAsync();
                            if(bytes3 ==null )
                            {
                                continue;
                            }
                            data =Encoding.UTF8.GetString(bytes3);

                            var person = await firebaseHelper.GetPersonByID(data);
                            var distance = getDistance(device.Rssi, -69);
                            if (data != "" && person!=null && distance <= 2) {
                                var location = await Geolocation.GetLastKnownLocationAsync();
                                Placemark placemark = null;
                                if (location != null)
                                {
                                    var Latitude = 0.01 ;
                                    var Longitude = 0.01;
                                    if (location.IsFromMockProvider)
                                    {
                                        //Put a message if detect a mock location.
                                    }
                                    else
                                    {
                                        Latitude = location.Latitude;
                                        Longitude = location.Longitude;
                                    }
                                    var placemarks = await Geocoding.GetPlacemarksAsync(Latitude, Longitude);

                                    placemark = placemarks?.FirstOrDefault();
                                   
                                }
                                NotifyUser = true;
                               var Interacteduserinfo= await App.Database.GetIntreactionInfo(person.Email);
                                if (Interacteduserinfo != null)
                                {
                                    Interacteduserinfo.DateTime = DateTime.Now;
                                    Interacteduserinfo.Distance = distance.ToString("0.00") + " M";
                                    Interacteduserinfo.Time = DateTime.UtcNow.ToLocalTime().ToString("h:mm tt");
                                    Interacteduserinfo.Adress1 = placemark.SubLocality + " , "  + placemark.Locality + " , " + placemark.SubAdminArea;
                                    Interacteduserinfo.Adress2 = placemark.AdminArea + " , " + placemark.CountryName + " , " + placemark.PostalCode;
                                    await App.Database.UpdateTimeLineRecord(Interacteduserinfo);
                                    MessagingCenter.Send<BleScannerService, string>(this, "RecordDetected", "RecordsUpdtaed");

                                }
                                else
                                {
                                    TimeLineModel TimeLine = new TimeLineModel();

                                    TimeLine.Email = person.Email;
                                    TimeLine.Name = person.Name;
                                    TimeLine.Picture = person.Picture;
                                    if (person.IsInfected)
                                    {
                                        TimeLine.TransportColor = "red";

                                    }
                                    else
                                    {
                                        TimeLine.TransportColor = "#76c2af";

                                    }
                                    TimeLine.Distance = distance.ToString("0.00") + " M";
                                    TimeLine.TransportType = "Walking";
                                    TimeLine.DateTime = DateTime.Now;
                                    TimeLine.Time = DateTime.UtcNow.ToLocalTime().ToString("h:mm tt");
                                    TimeLine.Adress1 = placemark.SubLocality + " , " + placemark.Locality + " , " + placemark.SubAdminArea;
                                    TimeLine.Adress2 = placemark.AdminArea + " , " + placemark.CountryName + " , " + placemark.PostalCode;


                                    await App.Database.AddTimeLineRecord(TimeLine);
                                    MessagingCenter.Send<BleScannerService, string>(this, "RecordDetected", "RecordsUpdtaed");
                                }
                           
                            }
                        }
                        await adapter.DisconnectDeviceAsync(device);
                        if (NotifyUser)
                        {
                            MessagingCenter.Send<BleScannerService, string>(this, "LiveEvent", "RecordsUpdtaed");
                            NotifyUser = false;
                        }
                    }

                }
                deviceList.Clear();
                Thread.Sleep(GlobalSettings.DeviceReadTimeInMilliseconds); 

            }

        }
    }
}
