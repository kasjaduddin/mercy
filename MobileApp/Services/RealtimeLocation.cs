using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using MobileApp.Models;

namespace MobileApp.Services
{
    public class RealtimeLocation
    {
        private static CurrentLocation? _currentLocation;

        public static CurrentLocation? CurrentLocation
        {
            get => _currentLocation;
        }

        public static async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.LocationAlways>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.LocationAlways>();

            return status;
        }

        private static async Task<Location?> GetLastKnownLocation()
        {
            Location? lastKnownLocation = new Location();
            PermissionStatus status = await CheckAndRequestLocationPermission();

            if (status == PermissionStatus.Granted)
            {
                lastKnownLocation = await Geolocation.GetLastKnownLocationAsync();

                if (lastKnownLocation != null)
                {
                    lastKnownLocation = await Geolocation.GetLocationAsync();
                }
            }

            return lastKnownLocation;
        }

        public static async Task GetCurrentLocation()
        {
            Location? lastKnownLocation = await GetLastKnownLocation();

            if (lastKnownLocation != null)
            {
                IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(lastKnownLocation.Latitude, lastKnownLocation.Longitude);
                Placemark? placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    _currentLocation = new CurrentLocation(placemark);
                }
            }
        }

        //private CancellationTokenSource? _cancelTokenSource;
        //private bool _isCheckingLocation;

        //public async Task GetCurrentLocation()
        //{
        //    try
        //    {
        //        _isCheckingLocation = true;

        //        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

        //        _cancelTokenSource = new CancellationTokenSource();

        //        Location? location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

        //        if (location != null)
        //            await GetGeocodeReverseData(location.Latitude, location.Longitude);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        _isCheckingLocation = false;
        //    }
        //}

        //private async Task<string> GetGeocodeReverseData(double latitude, double longitude)
        //{
        //    IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);

        //    Placemark? placemark = placemarks?.FirstOrDefault();

        //    if (placemark != null)
        //    {
        //        return
        //            $"AdminArea:       {placemark.AdminArea}\n" +
        //            $"CountryCode:     {placemark.CountryCode}\n" +
        //            $"CountryName:     {placemark.CountryName}\n" +
        //            $"FeatureName:     {placemark.FeatureName}\n" +
        //            $"Locality:        {placemark.Locality}\n" +
        //            $"PostalCode:      {placemark.PostalCode}\n" +
        //            $"SubAdminArea:    {placemark.SubAdminArea}\n" +
        //            $"SubLocality:     {placemark.SubLocality}\n" +
        //            $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
        //            $"Thoroughfare:    {placemark.Thoroughfare}\n";
        //    }
        //    return "";
        //}

        //public void CancelRequest()
        //{
        //    if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
        //        _cancelTokenSource.Cancel();
        //}
    }
}
