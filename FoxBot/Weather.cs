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
            Stream s = response.GetResponseStream();
            XmlReader xmlReader = XmlReader.Create(s);
            while (xmlReader.Read())
            {
                if (xmlReader.Name.Equals("current") && xmlReader.NodeType.Equals(XmlNodeType.Element) && !xmlReader.NodeType.Equals(XmlNodeType.EndElement))
                    while (xmlReader.Read() && !(xmlReader.NodeType.Equals(XmlNodeType.EndElement) && xmlReader.Name.Equals("current")))
                        if (xmlReader.NodeType.Equals(XmlNodeType.EndElement))
                            continue;
                        else
                            switch (xmlReader.Name.ToLower())
                            {
                                case "temp_f":
                                    xmlReader.Read();
                                    immediateWeather.temp = decimal.Parse(xmlReader.Value);
                                    break;
                                case "wind_mph":
                                    xmlReader.Read();
                                    immediateWeather.windSpeed = decimal.Parse(xmlReader.Value);
                                    break;
                                case "wind_dir":
                                    xmlReader.Read();
                                    immediateWeather.windDir = xmlReader.Value;
                                    break;
                                case "condition":
                                    while (xmlReader.Read() && !(xmlReader.NodeType.Equals(XmlNodeType.EndElement) && xmlReader.Name.Equals("condition")))
                                        if (xmlReader.Name.Equals("text") && xmlReader.NodeType.Equals(XmlNodeType.Element))
                                        {
                                            xmlReader.Read();
                                            immediateWeather.clouds = xmlReader.Value;
                                        }
                                    break;
                            }
                if (xmlReader.Name.Equals("location") && xmlReader.NodeType.Equals(XmlNodeType.Element) && !xmlReader.NodeType.Equals(XmlNodeType.EndElement))
                    while (xmlReader.Read() && !(xmlReader.NodeType.Equals(XmlNodeType.EndElement) && xmlReader.Name.Equals("location")))
                        if (xmlReader.NodeType.Equals(XmlNodeType.EndElement))
                            continue;
                        else
                            switch (xmlReader.Name.ToLower())
                            {
                                case "name":
                                    xmlReader.Read();
                                    immediateWeather.city = xmlReader.Value;
                                    break;
                                case "region":
                                    xmlReader.Read();
                                    immediateWeather.state = xmlReader.Value;
                                    break;
                                case "country":
                                    xmlReader.Read();
                                    immediateWeather.country = xmlReader.Value;
                                    break;
                            }
            }
        }
    }
}
