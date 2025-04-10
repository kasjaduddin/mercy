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
        private static ICharacteristic? activeCharacteristic = null;

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

                if (state == BluetoothState.On)
                {
                    Console.WriteLine("Bluetooth is ON");
                    ConnectToBluetoothDevices();
                }
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
                                StartBackgroundSubscription(device);
                                break;
                            }
                        }
                        catch (DeviceConnectionException ex)
                        {
                            Console.WriteLine($"Error connecting to device: {ex.Message}");
                            ConnectToBluetoothDevices();
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Already connected to a device.");

            }
        }

        public static void StartBackgroundSubscription(IDevice device)
        {
            Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine("Starting background subscription...");
                    await SubscribeToNotificationsAsync(device);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in background subscription: {ex.Message}");
                }
            });
        }

        private static async Task SubscribeToNotificationsAsync(IDevice device)
        {
            try
            {
                var services = await device.GetServicesAsync();
                var targetService = services.FirstOrDefault(s => s.Id.ToString() == "4fafc201-1fb5-459e-8fcc-c5c9c331914b");

                if (targetService != null)
                {
                    Console.WriteLine($"Found target service: {targetService.Id}");

                    var characteristics = await targetService.GetCharacteristicsAsync();
                    activeCharacteristic = characteristics.FirstOrDefault(c => c.Id.ToString() == "beb5483e-36e1-4688-b7f5-ea07361b26a8");

                    if (activeCharacteristic != null)
                    {
                        Console.WriteLine($"Found target characteristic: {activeCharacteristic.Id}");

                        activeCharacteristic.ValueUpdated += (s, e) =>
                        {
                            var data = e.Characteristic.Value;

                            if (data.Length >= 2)
                            {
                                ushort value = BitConverter.ToUInt16(data, 0);
                                App.HeartMonitor.AddData(value);
                            }
                            else
                            {
                                Console.WriteLine("Invalid data length for UInt16 conversion.");
                            }
                        };

                        await activeCharacteristic.StartUpdatesAsync();
                        Console.WriteLine("Subscribed to notifications.");
                    }
                    else
                    {
                        Console.WriteLine("Target characteristic not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Target service not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error subscribing to notifications: {ex.Message}");
            }
        }

        public static async Task StopDataCollection()
        {
            try
            {
                if (activeCharacteristic != null)
                {
                    await activeCharacteristic.StopUpdatesAsync();
                    activeCharacteristic.ValueUpdated -= (s, e) => { };
                    Console.WriteLine("Data collection stopped.");
                }
                else
                {
                    Console.WriteLine("No active characteristic to stop.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping data collection: {ex.Message}");
            }
        }
    }
}