using MobileApp.Components.Pages;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;

namespace MobileApp.Services
{
    public class BluetoothLowEnergy
    {
        private static BluetoothState state;
        private static IBluetoothLE ble = CrossBluetoothLE.Current;
        private static IAdapter adapter = CrossBluetoothLE.Current.Adapter;
        private static bool deviceConnected = false;
        private static List<IDevice> deviceList = new List<IDevice>();

        public static BluetoothState State
        {
            get => state;
        }

        public static IBluetoothLE Ble
        {
            get => ble;
        }

        public static IAdapter Adapter
        {
            get => adapter;
        }

        public static bool DeviceConnected
        {
            get => deviceConnected;
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

        public static async Task GetBluetoothStatus()
        {
            PermissionStatus status = await CheckAndRequestBluetoothPermission();

            if (status == PermissionStatus.Granted)
            {
                state = ble.State;

                ble.StateChanged -= OnBluetoothStateChanged;
                ble.StateChanged += OnBluetoothStateChanged;
            }
        }

        private static async void OnBluetoothStateChanged(object sender, BluetoothStateChangedArgs e)
        {
            Console.WriteLine($"The Bluetooth state changed to {e.NewState}");

            if (e.NewState == BluetoothState.On)
            {
                await ConnectToBluetoothDevices();
            }
        }

        private static bool CheckConnectedDevices()
        {
            adapter.DeviceConnected -= OnDeviceConnected;
            adapter.DeviceDisconnected -= OnDeviceDisconnected;

            adapter.DeviceConnected += OnDeviceConnected;
            adapter.DeviceDisconnected += OnDeviceDisconnected;

            return adapter.ConnectedDevices.Any(d => d.Name.Contains("Mercy") || d.Name.Contains("ECG"));
        }

        private static void OnDeviceConnected(object sender, DeviceEventArgs a)
        {
            if (a.Device.Name.Contains("Mercy") || a.Device.Name.Contains("ECG"))
            {
                deviceConnected = true;
                Console.WriteLine($"Device connected: {a.Device.Name}");
            }
        }

        private static void OnDeviceDisconnected(object sender, DeviceEventArgs a)
        {
            if (a.Device.Name.Contains("Mercy") || a.Device.Name.Contains("ECG"))
            {
                deviceConnected = false;
                Console.WriteLine($"Device disconnected: {a.Device.Name}");
                ConnectToBluetoothDevices();
            }
        }

        private static async Task ScanForDevices()
        {
            if (state == BluetoothState.On)
            {
                deviceConnected = CheckConnectedDevices();

                if (!deviceConnected)
                {
                    Console.WriteLine("Scanning for Bluetooth devices...");

                    deviceList.Clear();

                    adapter.DeviceDiscovered -= OnDeviceDiscovered;
                    adapter.DeviceDiscovered += OnDeviceDiscovered;

                    await adapter.StartScanningForDevicesAsync();
                }
            }
        }

        private static void OnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            if (!deviceList.Contains(e.Device) && (e.Device.Name?.Contains("Mercy") == true || e.Device.Name?.Contains("ECG") == true))
            {
                deviceList.Add(e.Device);
                Console.WriteLine($"Device discovered: {e.Device.Name} ({e.Device.Id})");
            }
        }

        public static async Task ConnectToBluetoothDevices()
        {
            if (!deviceConnected)
            {
                Console.WriteLine("No devices connected. Attempting to connect...");
                await ScanForDevices();

                foreach (var device in deviceList)
                {
                    if (device.Name.Contains("Mercy") || device.Name.Contains("ECG"))
                    {
                        try
                        {
                            Console.WriteLine($"Attempting connection to: {device.Name} ({device.Id})");

                            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                            await adapter.ConnectToDeviceAsync(device, cancellationToken: cts.Token);

                            deviceConnected = CheckConnectedDevices();
                            Console.WriteLine($"Connection status: {deviceConnected}");

                            if (deviceConnected)
                            {
                                Console.WriteLine($"Successfully connected to: {device.Name}");
                                break;
                            }
                        }
                        catch (DeviceConnectionException ex)
                        {
                            Console.WriteLine($"Error connecting to device: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Already connected to a device.");
            }
        }
    }
}
