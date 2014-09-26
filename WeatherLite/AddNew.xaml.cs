using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System.IO.IsolatedStorage;
using System.IO;

namespace WeatherLite
{
    public partial class AddNew : PhoneApplicationPage
    {
        string city;
        string country;
        int counter = 0;
        public AddNew()
        {
            InitializeComponent();
        }
        //this method is called when the Add button is pressed
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            rfile();// rfile method reads the state.txt file which stores the favourite place
            if (counter >= 1)//counter value is 1 when a favourite is already present
            {
                MessageBox.Show("You can add only one favourite");
            }
            else//if a favourite is not already present, then only call the ngetWeather method
            {
                //details from the text boxes
                city = ncity.Text;
                country = ncountry.Text;
                ngetWeather();// requests information for city, country
            }
        }

        //This method sends a request to the server for specified city and country data, if response returns an error code it means that the city or country is incorrect
        public void ngetWeather()
        {
            var naddress = "http://api.openweathermap.org/data/2.5/weather?q=" +city + "," +country; // address from where data will be fetched
            // var address = "http://api.openweathermap.org/data/2.5/weather?lat=35&lon=139";
            //var address = "http://api.openweathermap.org/data/2.5/weather?q=Kanpur,india";
            WebClient nclient = new WebClient(); //Web Client is required to fetch data from a web service
            Uri nuri = new Uri(naddress); // Constructing a uri from the address as URI is required by DownloadAsync method of WebClient Object
            nclient.DownloadStringCompleted += nclient_DownloadStringCompleted;
            nclient.DownloadStringAsync(nuri);
        }

        private void nclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Create the Json serializer and parse the response
            try
            {
                var obj1 = JsonConvert.DeserializeObject<Weath>(e.Result);
                this.DataContext = obj1;
                if (obj1.cod.ToString() == "404")//'404' cod is returned in case of unknown city or country
                {
                    MessageBox.Show("Unknown City/Country: Please enter city and country correctly");
                }
                else//if JSON response is correct, add the city, country and counter entry to state.txt file and save
                {
                    sfile();//creates and saves state.txt file
                }
            }
            catch
            {
                MessageBox.Show("Error : Server returned an incorrect response");// if server could not be reached or sent another error message
            }
            
        }

        //Gesture - Flick
        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction.ToString().Equals("Horizontal"))
            {

                if (e.HorizontalVelocity > 0)//right
                {
                    NavigationService.Navigate(new Uri("/More.xaml", UriKind.Relative));
                }
                else //Left
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }

            }
        }

        //Create state.txt file and save the entries into it
        private void sfile()
        {

            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            //create new file
            using (StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream("state.txt", FileMode.Create, FileAccess.Write, file)))
            {

                string text = city+","+country+","+counter;//text to be written to the state.txt file. Each value is separted by comma for easy retrieval
                sw.WriteLine(text);//Write to the file state.txt
                sw.Close();
                MessageBox.Show("Location has been added successfully. Taking you to the Favourites page.");// Tell user that the favourite is added
                
            }
            file.Dispose();
            NavigationService.Navigate(new Uri("/More.xaml", UriKind.Relative));//Take the user to Favourites page where he can refresh and see newly added location
        }

        //Method to find whether a favourite is already present
        private void rfile()
        {

            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (file.FileExists("state.txt"))// check if state.txt exists
            {
                IsolatedStorageFileStream fs = file.OpenFile("state.txt", FileMode.Open, FileAccess.Read);// open state.txt

                using (StreamReader sr = new StreamReader(fs))
                {

                    String str = sr.ReadLine();
                    String[] temp = str.Split(',');
                   // MessageBox.Show(temp[2]);
                    counter = Int32.Parse(temp[2]);//fetching counter value.
                    /* Ideally, just checking for file existence is enough to tell whether a favourite is present or not.
                     * The notion of counter will help if we go on to allow users to add more than one location
                     * */
                    sr.Close(); // Close Stream Reader
                }
                fs.Close(); // Close Isolated File Stream
            }
            else
            {
                counter = 0;
            }
            file.Dispose();// Release resources
        }
    }
}