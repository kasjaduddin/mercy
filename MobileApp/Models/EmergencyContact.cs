using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApp.Models
{
    public class EmergencyContact
    {
        private string _contactname;
        private string _phoneNumber;

        public string Username
        {
            get => _contactname;
            set => _contactname = value;
        }
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = value;
        }

        public EmergencyContact()
        {
            _contactname = String.Empty;
            _phoneNumber = String.Empty;
        }

        public EmergencyContact(string contactname, string phoneNumber)
        {
            _contactname = contactname;
            _phoneNumber = phoneNumber;
        }
    }
}
