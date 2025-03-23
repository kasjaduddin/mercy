using MobileApp.Models;

namespace MobileApp
{
    public partial class App : Application
    {
        public static User? CurrentUser { get; set; }

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
        }

        public static void LoadUserData(string userName, string userPhoneNumber)
        {
            CurrentUser = new User(userName, userPhoneNumber);
        }
    }
}
