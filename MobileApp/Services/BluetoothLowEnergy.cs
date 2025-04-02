using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApp.Services
{
    public class BluetoothLowEnergy
    {
    #if ANDROID
        public static async Task<PermissionStatus> CheckAndRequestBluetoothPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
            Console.WriteLine("Bluetooth permission status: " + status);
            if (status == PermissionStatus.Granted)
                return status;

            if (Permissions.ShouldShowRationale<Permissions.Bluetooth>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.Bluetooth>();
            Console.WriteLine("Bluetooth permission status: " + status);
            return status;
        }
    #endif
    }
}
