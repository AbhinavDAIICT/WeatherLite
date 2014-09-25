using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using Newtonsoft.Json;

namespace WeatherLite
{
    public partial class More : PhoneApplicationPage
    {
        string addr = null;
        public More()
        {
            InitializeComponent();
        }
        public void fgetWeather()
        {
           // var faddress = "http://api.openweathermap.org/data/2.5/weather?q="+city+","+country;
            WebClient fclient = new WebClient(); //Web Client is required to fetch data from a web service
            Uri furi = new Uri(addr); // Constructing a uri from the address as URI is required by DownloadAsync method of WebClient Object

            fclient.DownloadStringCompleted += fclient_DownloadStringCompleted;
            //client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCallback2); // it is called when download from remote source is complete
            fclient.DownloadStringAsync(furi);

        }

        private void fclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Create the Json serializer and parse the response
            var fobj = JsonConvert.DeserializeObject<Weath>(e.Result);
            this.DataContext = fobj;
            fvmintemp.Text = fobj.main.temp_min.ToString();
            fvmaxtemp.Text = fobj.main.temp_max.ToString();
            fvtemp.Text = fobj.main.temp.ToString();
            fvwind.Text = fobj.wind.speed.ToString();

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            frfile();
            fgetWeather();
        }

        private void frfile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

            IsolatedStorageFileStream fs = file.OpenFile("state.txt", FileMode.Open, FileAccess.Read);

            using (StreamReader sr = new StreamReader(fs))
            {

                String str = sr.ReadLine();
                String[] temp = str.Split(',');
                addr = "http://api.openweathermap.org/data/2.5/weather?q=" + temp[0] + "," + temp[1];
                fvcity.Text = temp[0];
                fvcountry.Text = temp[1];               
            }
        }

        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction.ToString().Equals("Horizontal"))
            {

                if (e.HorizontalVelocity > 0)// Go right
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
                else //Left
                {
                    NavigationService.Navigate(new Uri("/AddNew.xaml", UriKind.Relative));
                }

            }
        }

    }
}