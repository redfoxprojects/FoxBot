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
using System.Configuration;
using System.Windows;
using System.Net;
using FoxBot.ServiceReference1;

namespace FoxBot
{
    class Weather
    {
        private string currentLocation;

        public Weather(string location)
        {
            currentLocation = location;
        }

        public string GetWeather()
        {
            Service1Client srv = new Service1Client();
            if (currentLocation.Length.Equals(5))
            {
                int blah;
                int.TryParse(currentLocation, out blah);

                return Format(srv.GetData(blah.ToString()));
            }
            else if (currentLocation.Length.Equals(3))
            {
                return Format(srv.GetData(currentLocation));
            }
            else
                throw new Exception("zip not long enough");
        }

        private string Format(ImmediateWeather weather)
        {
            return string.Format("{0}°F wind {1} @ {2} MPH with {3} skies in {4}, {5}, {6}", new string[] { weather.temp.ToString(),
                                                                                                                weather.windDir.Trim(),
                                                                                                                weather.windSpeed.ToString(),
                                                                                                                weather.clouds.Trim(),
                                                                                                                weather.city.Trim(),
                                                                                                                weather.state.Trim(),
                                                                                                                weather.country.Trim()});
        }
    }
}