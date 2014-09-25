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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            rfile();
            if (counter >= 1)
            {
             MessageBox.Show("You can add only one favourite");
            }
            city = ncity.Text;
            country = ncountry.Text;
           // NavigationService.Navigate(new Uri("/MainPage.xaml?msg=" + ncity.Text+","+ncountry.Text, UriKind.Relative));
            ngetWeather();
        }

        public void ngetWeather()
        {
            var naddress = "http://api.openweathermap.org/data/2.5/weather?q="+city+","+country;
            WebClient nclient = new WebClient(); //Web Client is required to fetch data from a web service
            Uri nuri = new Uri(naddress); // Constructing a uri from the address as URI is required by DownloadAsync method of WebClient Object

            nclient.DownloadStringCompleted += nclient_DownloadStringCompleted;
            //client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCallback2); // it is called when download from remote source is complete
            nclient.DownloadStringAsync(nuri);

        }

        private void nclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Create the Json serializer and parse the response
            var nobj = JsonConvert.DeserializeObject<Weath>(e.Result);
            this.DataContext = nobj;
            // Populate the TextBoxes with the result fetched from JSON
            if (nobj.cod == 404)
            {

                MessageBox.Show("Unknown City/Country: Please enter city and country correctly");
            }
            if (counter == 0)
            {
                counter = 1;
                sfile();
            }
            else
            {
                MessageBox.Show("You can add only one favourite");

            }
        }

        private void sfile()
        {

            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            //create new file
            using (StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream("state.txt", FileMode.Create, FileAccess.Write, file)))
            {

                string text = city+","+country+","+counter;

                sw.WriteLine(text);

                sw.Close();

                MessageBox.Show("Location has been added successfully");

            }
        }

        private void rfile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

            IsolatedStorageFileStream fs = file.OpenFile("state.txt", FileMode.Open, FileAccess.Read);

            using (StreamReader sr = new StreamReader(fs))
            {

                String str = sr.ReadLine();
                String[] temp = str.Split(',');
                MessageBox.Show(temp[2]);
                counter = Int32.Parse(temp[2]);

            }
        }
    }
}