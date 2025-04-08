using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApp.Models
{
    public class CurrentLocation
    {
        private string _latitude;
        private string _longitude;
        private string? _street;
        private string? _subLocality;
        private string? _locality;
        private string? _subAdminArea;
        private string? _adminArea;
        private string? _postalCode;

        public string Latitude
        {
            get => _latitude;
            set => _latitude = value;
        }

        public string Longitude
        {
            get => _longitude;
            set => _longitude = value;
        }

        public string? Street
        {
            get => _street;
            set => _street = value;
        }

        public string? SubLocality
        {
            get => _subLocality;
            set => _subLocality = value;
        }

        public string? Locality
        {
            get => _locality;
            set => _locality = value;
        }

        public string? SubAdminArea
        {
            get => _subAdminArea;
            set => _subAdminArea = value;
        }

        public string? AdminArea
        {
            get => _adminArea;
            set => _adminArea = value;
        }

        public string? PostalCode
        {
            get => _postalCode;
            set => _postalCode = value;
        }

        public CurrentLocation()
        {
            _latitude = String.Empty;
            _longitude = String.Empty;
            _street = String.Empty;
            _subLocality = String.Empty;
            _locality = String.Empty;
            _subAdminArea = String.Empty;
            _adminArea = String.Empty;
            _postalCode = String.Empty;
        }

        public CurrentLocation(Placemark placemark)
        {
            _latitude = placemark.Location.Latitude.ToString();
            _longitude = placemark.Location.Longitude.ToString();
            _street = placemark.Thoroughfare;
            _subLocality = placemark.SubLocality;
            _locality = placemark.Locality;
            _subAdminArea = placemark.SubAdminArea;
            _adminArea = placemark.AdminArea;
            _postalCode = placemark.PostalCode;
        }
    }
}
