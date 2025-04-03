using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Threading.Tasks;
#if ANDROID
using Android.Content;
#endif

namespace MobileApp.Services
{
    public class BluetoothLowEnergy
    {
        private static BluetoothState state;
        private static IBluetoothLE ble = CrossBluetoothLE.Current;
        private static IAdapter adapter = CrossBluetoothLE.Current.Adapter;
        private static List<IDevice> deviceList = new List<IDevice>();

        public static BluetoothState State
        {
            get => state;
        }

        public static async Task<PermissionStatus> CheckAndRequestBluetoothPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();

            if (status == PermissionStatus.Granted)
                return status;

            if (Permissions.ShouldShowRationale<Permissions.Bluetooth>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.Bluetooth>();

            return status;
        }

        private static async Task GetBluetoothStatus()
        {
            PermissionStatus status = await CheckAndRequestBluetoothPermission();
            
            if (status == PermissionStatus.Granted)
            {
                state = ble.State;
                
                ble.StateChanged += async (s, e) =>
                {
                    Console.WriteLine($"The bluetooth state changed to {e.NewState}");
                    await ConnectToBluetoothDevices();
                };
            }
        }

        private static void OpenBluetoothSettings()
        {
            bool connectedDevices = CheckConnectedDevices();
#if ANDROID
            if (state == BluetoothState.On && !connectedDevices)
            {
                try
                {
                    var intent = new Intent(Android.Provider.Settings.ActionBluetoothSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error opening Bluetooth settings: {ex.Message}");
                }
            }
#endif
        }

        private static bool CheckConnectedDevices()
        {
            adapter.DeviceConnected += (s, a) =>
            {
                deviceList.Add(a.Device);
            };

            foreach (var device in deviceList)
            {
                if (device.Name.Contains("Mercy"))
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task ConnectToBluetoothDevices()
        {
            await GetBluetoothStatus();
            OpenBluetoothSettings();
        }
    }
}
