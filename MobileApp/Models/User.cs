namespace MobileApp.Models
{
    public class User
    {
        private string _username;
        private string _phoneNumber;
        private bool _loggedIn;

        public string Username
        {
            get => _username;
            set => _username = value;
        }
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = value;
        }

        public bool LoggedIn
        {
            get => _loggedIn;
            set => _loggedIn = value;
        }

        public User()
        {
            _username = String.Empty;
            _phoneNumber = String.Empty;
            _loggedIn = false;
        }

        public User(string username, string phoneNumber)
        {
            _username = username;
            _phoneNumber = phoneNumber;
            _loggedIn = true;
        }
    }
}
