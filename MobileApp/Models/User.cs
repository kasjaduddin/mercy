namespace MobileApp.Models
{
    public class User
    {
        private string _username;
        private string _phoneNumber;

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

        public User()
        {
            _username = "Default User";
            _phoneNumber = "+62xxxxxxxxxxx";
        }

        public User(string username, string phoneNumber)
        {
            _username = username;
            _phoneNumber = phoneNumber;
        }
    }
}
