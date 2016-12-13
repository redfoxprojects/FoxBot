using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Xml;
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

        public Weather(string location)
        {
            currentLocation = location;
            request = WebRequest.Create(string.Format("http://api.worldweatheronline.com/free/v1/weather.ashx?q={0}&key=5ay9y7urmtdcm2v82p34qbzg&num_of_days=1", currentLocation));
            immediateWeather = new ImmediateWeather();
        }

        public ImmediateWeather GetWeather()
        {
            int blah;
            if (!int.TryParse(currentLocation, out blah) && !currentLocation.Length.Equals(5))
            {
                throw new Exception("zip not long enough");
            }
            object o = new object();
            SetWeather(request.GetResponse());
            return immediateWeather;

        }

        public ImmediateWeather ImmediateWeather
        {
            get { return immediateWeather; }
            set { immediateWeather = value; }
        }

        private void SetWeather(WebResponse response)
        {
            Stream s = response.GetResponseStream();
            XmlReader xmlReader = XmlReader.Create(s);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType.Equals(XmlNodeType.EndElement) && xmlReader.Name.Equals("current_condition"))
                    Console.Out.WriteLine("bad request");
                else if (xmlReader.NodeType.Equals(XmlNodeType.Element))
                    switch (xmlReader.Name.ToLower())
                    {
                        case "temp_f":
                            xmlReader.Read();
                            immediateWeather.temp = decimal.Parse(xmlReader.Value);
                            break;
                        case "windspeedmiles":
                            xmlReader.Read();
                            immediateWeather.windSpeed = decimal.Parse(xmlReader.Value);
                            break;
                        case "winddir16point":
                            xmlReader.Read();
                            immediateWeather.windDir = xmlReader.Value;
                            break;
                        case "weatherdesc":
                            xmlReader.Read();
                            immediateWeather.clouds = xmlReader.Value;
                            break;
                    }
            }
        }
    }
}
