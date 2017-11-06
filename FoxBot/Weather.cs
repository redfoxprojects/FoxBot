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
using WeatherServices;

namespace FoxBot
{
    class Weather
    {
        private string currentLocation;

        public Weather(string location)
        {
            currentLocation = location;
        }

        public ImmediateWeather GetWeather()
        {
            WeatherService srv = new WeatherService();
            if (currentLocation.Length.Equals(5))
            {
                int blah;
                int.TryParse(currentLocation, out blah);

                return srv.GetWeather(srv.GetStream(blah.ToString()));
            }
            else if (currentLocation.Length.Equals(3))
            {
                return srv.GetWeather(srv.GetStream(currentLocation));
            }
            else
                throw new Exception("zip not long enough");
        }
    }
}