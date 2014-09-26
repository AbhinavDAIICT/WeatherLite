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
        //requests weather data for the 'addr' generated in the frfile() method
        public void fgetWeather()
        {
            WebClient fclient = new WebClient(); //Web Client is required to fetch data from a web service
            Uri furi = new Uri(addr); // Constructing a uri from the address as URI is required by DownloadAsync method of WebClient Object
            fclient.DownloadStringCompleted += fclient_DownloadStringCompleted;
            fclient.DownloadStringAsync(furi);

        }

        private void fclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Create the Json serializer and parse the response
            var fobj = JsonConvert.DeserializeObject<Weath>(e.Result);
            this.DataContext = fobj;
            //Populate the fields on Favourites(Maore.xaml) page
            fvmintemp.Text = fobj.main.temp_min.ToString();
            fvmaxtemp.Text = fobj.main.temp_max.ToString();
            fvtemp.Text = fobj.main.temp.ToString();
            fvwind.Text = fobj.wind.speed.ToString();

        }

        //Favouries data is loaded when Load Favourite button is clicked
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (frfile())//fgetWeather is called only if frfile() method returns true
            {
                fgetWeather();
            }
        }

        /*frfile() method reads the state.txt file from Isolated Storage.
         * state.txt is created when a new favourite is created and deleted when favourite is removed
         * if state.txt exists, this means that a favourite is already present.
         * state.txt contains "city,country,count"
         * city and country are used as a part of addr
         * */
        private bool frfile()
        {
              IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                if (file.FileExists("state.txt"))// checking if state.txt exists
                {
                    IsolatedStorageFileStream fs = file.OpenFile("state.txt", FileMode.Open, FileAccess.Read);// open file to read

                    using (StreamReader sr = new StreamReader(fs))// create StreamReader to get the stream from the file
                    {
                        String str = sr.ReadLine();//reading the whole file in a single string - str
                        String[] temp = str.Split(',');//Splitting str using comma delimitter.temp[0]->city,temp[1]->country,temp[2]->count
                        addr = "http://api.openweathermap.org/data/2.5/weather?q=" + temp[0] + "," + temp[1];//constructing url to be passed
                        //populating city and country information, rest is updated from JSON response
                        fvcity.Text = temp[0];
                        fvcountry.Text = temp[1];
                        sr.Close();//Free resources
                    }
                    fs.Close();
                    file.Dispose();
                    return true;//state.txt exists
                }
                else
                {
                    MessageBox.Show("No Favourites added yet. Click Add New button or Swipe Left to add a Favourite");// state.txt not exist. No favourite
                    return false;
                }
            
        }

        //Gesture Flick(or swipe)
        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction.ToString().Equals("Horizontal"))
            {

                if (e.HorizontalVelocity > 0)// right
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
                else //Left
                {
                    NavigationService.Navigate(new Uri("/AddNew.xaml", UriKind.Relative));
                }

            }
        }

        //method to remove a favourite
        private void remove_Click(object sender, EventArgs e)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (file.FileExists("state.txt"))// if favourite is present
            {
                //delete file
                file.DeleteFile("state.txt");
                file.Dispose();
                //set values to 0
                fvmintemp.Text = "0";
                fvmaxtemp.Text = "0";
                fvtemp.Text = "0";
                fvwind.Text = "0";
                //Display 'No Favourite' in city and country 
                fvcity.Text = "No Favourites";
                fvcountry.Text = "No Favourites";

            }
            else//user clicks remove button even if no favourite was present
            {
                MessageBox.Show("No Favourite has been added yet. Taking you to Add New page");
                NavigationService.Navigate(new Uri("/AddNew.xaml", UriKind.Relative));//Takes the user to AddNew page where he can add a new Favourite
            }
        }

        //This method is called when the user clicks the new button on app bar
        private void add_new_Click(object sender, EventArgs e)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (file.FileExists("state.txt"))// if file exists, user must be told that he can add max of one favourite
            {
                MessageBox.Show("Please remove the existing Favourite first. You can have only one Favourite at a time");

            }
            else {//if no favourite is added, take the user to AddNew page
                NavigationService.Navigate(new Uri("/AddNew.xaml", UriKind.Relative));
            }

        }

    }
}