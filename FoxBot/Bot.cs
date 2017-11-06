using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Runtime;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using IrcDotNet;
using IrcDotNet.Collections;
using IrcDotNet.Ctcp;
using System.Text.RegularExpressions;
using WeatherServices;

namespace FoxBot
{
    public class Bot
    {
        //private Client client;
        private IrcDotNet.IrcClient client = new IrcClient();
        private bool isQuitting = false;
        private SqlCommand sqlCom;
        private const string CHANNEL_PREFIX = "#";
        protected const string REALNAME = "foxbot";
        protected const string USERNAME = "foxbot";
        private string channel = null;
        private string server = string.Empty;

        public delegate void DataReceivedEventHandler(object sender, EventArgs e);
        public event DataReceivedEventHandler DataReceived;

        public Bot(string nick, string channel, string server)
        {
            this.Nick = nick;
            this.Channel = channel;
            this.server = server;
            sqlCom = new SqlCommand(string.Empty, new SqlConnection(ConfigurationManager.ConnectionStrings["foxbot"].ConnectionString));
        }

        public string Nick
        {
            get;
            set;
        }

        public string Channel
        {
            get
            {
                return channel;
            }
            set
            {
                if (value.StartsWith(CHANNEL_PREFIX))
                    channel = value;
                else
                    channel = CHANNEL_PREFIX + value;
            }
        }

        public void Connect()
        {
            this.client = new IrcClient();
            this.client.ErrorMessageReceived += client_ErrorMessageReceived;
            this.client.Connected += client_Connected;
            this.client.RawMessageReceived += client_RawMessageReceived;
            this.client.ConnectFailed += client_ConnectFailed;
            this.client.MotdReceived += client_MotdReceived;
            this.client.Error += client_Error;
            this.client.ProtocolError += client_ProtocolError;
            this.client.Disconnected += client_Disconnected;
            this.client.NetworkInformationReceived += client_NetworkInformationReceived;
            this.client.ClientInfoReceived += client_ClientInfoReceived;
            this.client.ValidateSslCertificate += client_ValidateSslCertificate;
            IrcUserRegistrationInfo serviceReg = new IrcUserRegistrationInfo();
            serviceReg.RealName = REALNAME;
            serviceReg.UserName = USERNAME;
            serviceReg.NickName = this.Nick;
            serviceReg.Password = "";
            this.client.Connect(this.server, 6667, false, serviceReg);

            while(!isQuitting)
                Thread.Sleep(10000);
        }

        private void LocalUser_JoinedChannel(object sender, IrcChannelEventArgs e)
        {
            this.client.LocalUser.SendMessage(this.client.Channels[0], "I AM FOXBOT! :V");
        }

        void client_ValidateSslCertificate(object sender, IrcValidateSslCertificateEventArgs e)
        {
            
        }

        void client_ClientInfoReceived(object sender, EventArgs e)
        {
            this.client.LocalUser.JoinedChannel += LocalUser_JoinedChannel;
            System.Console.Out.WriteLine(e.ToString());
        }

        void client_NetworkInformationReceived(object sender, EventArgs e)
        {
            System.Console.Out.WriteLine(e.ToString());
        }

        void client_Disconnected(object sender, EventArgs e)
        {
            isQuitting = true;
        }

        void client_ProtocolError(object sender, IrcProtocolErrorEventArgs e)
        {
            System.Console.Out.WriteLine(e.Message.ToString());
        }

        void client_Error(object sender, IrcErrorEventArgs e)
        {
            System.Console.Out.WriteLine(e.Error.ToString());
        }

        void client_ErrorMessageReceived(object sender, IrcErrorMessageEventArgs e)
        {
            System.Console.Out.WriteLine(e.Message.ToString());
        }

        void client_MotdReceived(object sender, EventArgs e)
        {
            this.client.SendRawMessage("JOIN :" + this.Channel);//JOIN :#channel //PRIVMSG #channel :
        }

        void client_ConnectFailed(object sender, IrcErrorEventArgs e)
        {
            System.Console.Out.WriteLine(e.Error.ToString());
        }

        void client_RawMessageReceived(object sender, IrcRawMessageEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(e.Message.Parameters[1]))
            {
                if (this.client.Channels.Count == 0)
                    return;
                else
                    Listen(e.Message);
            }
            LogMessage(e.RawContent);
        }

        private void LogMessage(string s)
        {
            try
                {
                    sqlCom.CommandType = CommandType.StoredProcedure;
                    sqlCom.CommandText = "AddRawMessage";
                    sqlCom.Parameters.Clear();
                    sqlCom.Parameters.Add(new SqlParameter("@message", s));
                    sqlCom.Connection.Open();
                    sqlCom.ExecuteNonQuery();
                }
                catch (Exception ex) { Console.Out.WriteLine(ex.Message); }
                finally 
                { 
                    sqlCom.Connection.Close();
                    System.Console.Out.WriteLine(s);
                }
        }

        void client_Connected(object sender, EventArgs e)
        {
            System.Console.Out.WriteLine(e.ToString());
        }

        public void SendMessage(string message)
        {
            this.client.LocalUser.SendMessage(this.client.Channels[0], message);
        }

        public void Listen(IrcClient.IrcMessage ircMessage)
        {
            string message = ircMessage.Parameters[1];
            if (message.ToLower().StartsWith(this.Nick.ToLower()))
            {
                string receivedMessageNick = ircMessage.Parameters[0];
                Regex reg = new Regex(@"[\w]+");
                int outvar = 0;
                string zipCode = string.Empty;
                foreach (Match m in reg.Matches(message))
                    switch (m.Value.ToLower())
                    {
                        case "weather":
                            Match zip = m.NextMatch();
                            if (zip != null)
                                if(zip.Length == 5 && int.TryParse(zip.Value, out outvar))
                                    zipCode = outvar.ToString();
                                else if (zip.Length == 3)
                                    zipCode = zip.ToString();
                            Weather w = new Weather(zipCode);
                            ParameterizedThreadStart pts = new ParameterizedThreadStart(ProcessRequest);
                            Thread t = new Thread(pts);
                            t.Start(w);
                            break;
                        case "leave":
                            if (ircMessage.Source.Name.ToLower().Equals("alex"))
                                client.Quit("D:");
                            break;
                    }
                return;
            }
        }

        protected void ProcessRequest(object o)
        {
            try
            {
                Weather w = (Weather)o;
                ImmediateWeather weather = w.GetWeather();
                SendMessage(string.Format("{0}°F wind {1} @ {2} MPH with {3} skies in {4}, {5}, {6}", new string[] { weather.temp.ToString(),
                                                                                                                weather.windDir.Trim(),
                                                                                                                weather.windSpeed.ToString(),
                                                                                                                weather.clouds.Trim(),
                                                                                                                weather.city.Trim(),
                                                                                                                weather.state.Trim(),
                                                                                                                weather.country.Trim()}));
            }
            catch (Exception e)
            {
                System.Console.Out.WriteLine(e);
            }
            finally
            {
                
            }
        }

        protected void OnData()
        {
            this.DataReceived(this, EventArgs.Empty);
        }
    }
}