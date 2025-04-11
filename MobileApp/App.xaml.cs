using MobileApp.Models;

namespace MobileApp
{
    public partial class App : Application
    {
        public static User? CurrentUser { get; set; }
        public static EmergencyContact? EmergencyContact { get; set; }

        public static readonly HeartConditionMonitor HeartMonitor = new HeartConditionMonitor();

        public App()
        {
            InitializeComponent();

            LoadData();

            MainPage = new MainPage();
        }

        // Initialize User from JSON file
        private void LoadData()
        {
            if (Preferences.ContainsKey("username"))
            {
                string userName = Preferences.Get("username", "");
                string userPhoneNumber = Preferences.Get("user_phone_number", "");

                LoadUserData(userName, userPhoneNumber);
            }
            else
            {
                CurrentUser = new User();
            }

            if (Preferences.ContainsKey("contact_name"))
            {
                string contactName = Preferences.Get("contact_name", "");
                string contactPhoneNumber = Preferences.Get("contact_phone_number", "");

                LoadContactsData(contactName, contactPhoneNumber);
            }
            else
            {
                EmergencyContact = new EmergencyContact();
            }
        }

        public static void LoadUserData(string userName, string userPhoneNumber)
        {
            CurrentUser = new User(userName, userPhoneNumber);
        }

        public static void LoadContactsData(string contactName, string contactPhoneNumber)
        {
            EmergencyContact = new EmergencyContact(contactName, contactPhoneNumber);
        }
    }
}
