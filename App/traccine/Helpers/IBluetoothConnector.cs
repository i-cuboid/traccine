using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace traccine.Helpers
{
    public interface IBluetoothConnector
    {
        Task<bool> EnableBluetooth(bool isenable);
        Task<bool> GetBluetoothStatus();
    }

}
