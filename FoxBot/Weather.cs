using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using System.Windows;
using System.Net;

namespace FoxBot
{
    class Weather
    {
        private string currentLocation;
        private WebRequest request;
        private ImmediateWeather immediateWeather;
        private const string URL_START = "https://api.apixu.com/v1/current.xml";
        private const string URL_KEY = "ff436f1b91a7474aab4223122170710";
        private const string URL = URL_START + "?q={0}&key=" + URL_KEY; //+ "&num_of_days=1&includelocation=yes";

        public Weather(string location)
        {
            currentLocation = location;
            request = WebRequest.Create(string.Format(URL, currentLocation));
            immediateWeather = new ImmediateWeather();
        }

        public ImmediateWeather GetWeather()
        {
            
            if (currentLocation.Length.Equals(5))
            {
                int blah;
                int.TryParse(currentLocation, out blah);
                SetWeather(request.GetResponse());
            }
            else if (currentLocation.Length.Equals(3))
            {
                SetWeather(request.GetResponse());
            }
            else
                throw new Exception("zip not long enough");
            
            return immediateWeather;

        }

        public ImmediateWeather ImmediateWeather
        {
            get { return immediateWeather; }
            set { immediateWeather = value; }
        }

        private void SetWeather(WebResponse response)
        {
            XPathNavigator nav = new XPathDocument(response.GetResponseStream()).CreateNavigator();   
            XPathNodeIterator itr;
            (itr = nav.Select("/root/current/temp_f")).MoveNext();
            immediateWeather.temp = decimal.Parse(itr.Current.Value);
            (itr = nav.Select("/root/current/wind_mph")).MoveNext();
            immediateWeather.windSpeed = decimal.Parse(itr.Current.Value);
            (itr = nav.Select("/root/current/wind_dir")).MoveNext();
            immediateWeather.windDir = itr.Current.Value;
            (itr = nav.Select("/root/current/condition/text")).MoveNext();
            immediateWeather.clouds = itr.Current.Value;
            (itr = nav.Select("/root/location/name")).MoveNext();
            immediateWeather.city = itr.Current.Value;
            (itr = nav.Select("/root/location/region")).MoveNext();
            immediateWeather.state = itr.Current.Value;
            (itr = nav.Select("/root/location/country")).MoveNext();
            immediateWeather.country = itr.Current.Value;

        }
    }
}