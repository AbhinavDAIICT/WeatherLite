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
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Device.Location;
using System.Windows.Media;
using Microsoft.Phone.Net.NetworkInformation;

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
            this.Loaded += MainPage_Loaded;
        }
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            getLoc(); // Start searching for the user location as soon as the page has finished loading. Comment this if you want to use only refresh button
            //getWeather();
        }

        //Code for add new button in the application bar
        private void addnew_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddNew.xaml", UriKind.Relative)); //Navigate to AddNew.xaml page where new place can be added
        }

        // This is called when 'Refresh Button is clicked
        private void More_Button_Click(object sender, RoutedEventArgs e)
        {
            if (netEnabled() == true)//Checking for internet connectivity
            {              
                getLoc();
            }
            else
            {
                MessageBox.Show("Please enable Data Connection or connect to WiFi");//Prompt the user to switch on internet connection
            }
        }

        // Checking if Location Service is On or not. Wrote this method but its not required
        public bool IsLocationServiceEnabled
        {
            get
            {
                Geolocator locationservice = new Geolocator();
                if (locationservice.LocationStatus == PositionStatus.Disabled)
                {
                    MessageBox.Show("The app needs to know your current location. Please switch on location from Settings.");
                    return false;
                }
                return true;
            }
        }

        // getLoc method retrieves the current Latitude and Longitude in the lati and longi variables. lati and longi are passed in the query to the web service to fetch data about the current location
        private async void getLoc()
        {
            // Getting current location of the user
            Geolocator geolocator = new Geolocator();
            MessageBox.Show("Please wait while we find your location");//Telling the user that we are searching for his location as it takes time

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );
                lati = geoposition.Coordinate.Latitude.ToString("0.00");//current latitude
                longi = geoposition.Coordinate.Longitude.ToString("0.00");//current longitude
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    //This exception occurs when the Location service in user's device is off
                    MessageBox.Show("The app needs to know your current location. Please switch on location from Settings.");
                }
                //else 
                {
                    MessageBox.Show("Sorry, we could not fetch your current location");
                }
            }
            
            /*Since we need to pass the lati and longi values to get information about our current location weather,
             * We must call the getWeather() method after the lati and longi values are available
             * So introducing a delayusing count
             * */
            int count = 0;
            while (lati == null && count < 1000)
            {
                count++;
            }
            if (count == 1000)
            {
                MessageBox.Show("Sorry, we could not fetch your current location");
            }
            getWeather();//This method fetches the data from OpenWeatherMap API
        }

        // Flick Gesture code
        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction.ToString().Equals("Horizontal"))
            {

                if (e.HorizontalVelocity > 0)// right
                {
                    NavigationService.Navigate(new Uri("/AddNew.xaml", UriKind.Relative));//Navigate to AddNew.xaml
                }else //Left

                {

                    NavigationService.Navigate(new Uri("/More.xaml", UriKind.Relative));//Navigate to More.xaml where Favourite place is displayed
                }

            }   
        }

        //Checking for Wifi or Data Connection
        public bool netEnabled()
        {
            if (DeviceNetworkInformation.IsCellularDataEnabled.ToString() == "True" || DeviceNetworkInformation.IsWiFiEnabled.ToString() == "True") { return true; }
            else { return false; }
        }

        // getWeather method requests for the weather information 
        public void getWeather()
        {
            var address = "http://api.openweathermap.org/data/2.5/weather?lat="+lati+"&lon="+longi; // address from where data will be fetched
            //comment the above line and uncomment the below line to fetch data for London,uk(i.e city,country) along with authorised key
            //var address = "https://api.openweathermap.org/data/2.5/weather?q=London,uk&APPID=603d8ba0e395ad677b0465f5540785ff";
            WebClient client = new WebClient(); //Web Client is required to fetch data from a web service
            client.Headers["Accept-Encoding"]="*";//tried to resolve an exception by accepting all types of encodings
            Uri uri = new Uri(address); // Constructing a uri from the address as URI is required by DownloadAsync method of client Object
            client.DownloadStringCompleted += client_DownloadStringCompleted;
            client.DownloadStringAsync(uri);
           
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Create the Json serializer and parse the response
            try
            {
                var obj = JsonConvert.DeserializeObject<Weath>(e.Result);//Weath class contains the classes generated from Json2C# converter
                this.DataContext = obj;
                // Populate the TextBoxes with the result fetched from JSON
                placeHead.Text = obj.name.ToString();
                temp.Text = obj.main.temp.ToString();
                country.Text = obj.sys.country;
                minTemp.Text = obj.main.temp_min.ToString();
                maxTemp.Text = obj.main.temp_max.ToString();
                humidity.Text = obj.main.humidity.ToString();
            }
            catch
            {
                //An exception occurs when the server returns "Not Found" message and obj is null
                MessageBox.Show("Sorry, the server returned an error message.");
            }
        }
       }
}