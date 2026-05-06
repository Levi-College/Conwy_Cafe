using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;

namespace Conwy_Cafe_Admin_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Setting up the API base URL as a static variable that can be accessed throughout the application.
        // This allows for easy configuration and maintenance of the API endpoint, as it can be changed in one place without needing to update multiple instances of the URL throughout the codebase.
        public static readonly HttpClient Http = new HttpClient
        {
            // Set the base address for the HttpClient to the specified URL.
            // It is the enndpoint for the API that the application will be communicating with.
            // This allows for easier and more consistent API calls throughout the application (as it is already defined and "BaseAddress" can be used).
            BaseAddress = new Uri("https://localhost:7008") 
        };
    
    }

}
