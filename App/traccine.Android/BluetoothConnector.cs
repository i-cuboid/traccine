using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using traccine.Droid;
using traccine.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(BluetoothConnector))]
namespace traccine.Droid
{
    public class BluetoothConnector : IBluetoothConnector
    {
        public BluetoothAdapter bluetoothAdapter { get; set; }
        public BluetoothConnector()
        {
            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
        }
        public async Task<bool> EnableBluetooth(bool isenable)
        {
       
            Boolean isEnabled = bluetoothAdapter.IsEnabled;
            if (isenable && !isEnabled)
            {
                return bluetoothAdapter.Enable();
            }
            else if (!isenable && isEnabled)
            {
                return bluetoothAdapter.Disable();
            }
            // No need to change bluetooth state
            return true;
        }

        public async Task<bool> GetBluetoothStatus()
        {
            return bluetoothAdapter.IsEnabled;
        }
    }
}