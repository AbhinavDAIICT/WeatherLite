using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WeatherLite.Resources;
using Windows.Devices.Geolocation;
using Newtonsoft.Json;

namespace WeatherLite
{
    public partial class MainPage : PhoneApplicationPage
    {
        string lati;
        string longi;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void addnew_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddNew.xaml", UriKind.Relative));
        }

        private void More_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/More.xaml", UriKind.Relative));
        }

        // getLoc method retrieves the current Latitude and Longitude in the lati and longi variables
        private async void getLoc()
        {
            // Getting current location of the user
            Geolocator geolocator = new Geolocator();
            try
            {
               
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );
                lati = geoposition.Coordinate.Latitude.ToString("0.00");
                longi = geoposition.Coordinate.Longitude.ToString("0.00");
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                }
                //else
                {
                    // something else happened acquring the location
                }
            }
        }

        // getWeather method gets the weather information 
        public void getWeather()
        {
            var address = "http://api.openweathermap.org/data/2.5/weather?lat=" + lati + "&lon=" + longi; // address from where data will be fetched
            WebClient client = new WebClient(); //Web Client is required to fetch data from a web service
            Uri uri = new Uri(address); // Constructing a uri from the address as URI is required by DownloadAsync method of WebClient Object

            // Specify that the DownloadStringCallback2 method gets called 
            // when the download completes.
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCallback); // it is called when download from remote source is complete
            client.DownloadStringAsync(uri);
        }

        private void DownloadStringCallback(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var rootObject = JsonConvert.DeserializeObject<RootObject>(e.Result);
                foreach (var name in rootObject.weather)
                {

                    //TextBlock b1 = new TextBlock();
                    dbg.Text = name.id.ToString();
                }
            }
            catch
            {
                dbg.Text = e.ToString();
            }


        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}