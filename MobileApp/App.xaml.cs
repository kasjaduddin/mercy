using MobileApp.Models;
using System.Text.Json;

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
            if (Preferences.ContainsKey("UserName"))
            {
                string userName = Preferences.Get("UserName", "");
                string userPhoneNumber = Preferences.Get("UserPhoneNumber", "");

                CurrentUser = new User(userName, userPhoneNumber);
            }
            else
            {
                CurrentUser = new User();
            }
        }
    }
}
