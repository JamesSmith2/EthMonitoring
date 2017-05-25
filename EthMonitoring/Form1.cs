using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EthMonitoring
{
    public partial class Form1 : Form
    {
        private BackgroundWorker bw;
        private Boolean Monitoring = false;
        private MySettings settings = MySettings.Load();

        class EthMonJsonTemplate
        {
            public int id { get; set; }
            public string error { get; set; }
            public List<string> result { get; set; }
        }
        
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
    
            this.bw = new BackgroundWorker();
            this.bw.DoWork += new DoWorkEventHandler(monitoringHosts);

            // Generate dictonary if needed
            if (settings.hosts == null)
            {
                settings.hosts = new Dictionary<string, string>();
            }

            // Set token value
            tokenField.Text = settings.accessToken;

            if (settings.hosts != null && settings.hosts.Count > 0)
            {
                foreach(KeyValuePair<string, string> entry in settings.hosts)
                {
                    ListViewItem newHost = new ListViewItem(entry.Value);

                    newHost.SubItems.Add(entry.Key); // Name
                    newHost.SubItems.Add("Initializing.."); // ETH HASHRATE
                    newHost.SubItems.Add("Initializing.."); // DCR HASHRATE
                    newHost.SubItems.Add("Initializing.."); // TEMPERATURES
                    newHost.SubItems.Add(""); // VERSION

                    hostsList.Items.Add(newHost);
                }

                if(tokenField.Text.Length > 0)
                {
                    startMonitoringMiners();
                }
            }
        }

        private void addhost_Click(object sender, EventArgs e)
        {
            try
            {
                if (hostField.Text != "" && hostName.Text != "")
                {
                    ListViewItem newHost = new ListViewItem(hostField.Text);

                    newHost.SubItems.Add(hostName.Text); // Hostname
                    newHost.SubItems.Add("Initializing.."); // ETH HASHRATE
                    newHost.SubItems.Add("Initializing.."); // DCR HASHRATE
                    newHost.SubItems.Add("Initializing.."); // TEMPERATURES
                    newHost.SubItems.Add(""); // VERSION

                    hostsList.Items.Add(newHost);

                    settings.hosts.Add(hostName.Text, hostField.Text);
                    settings.Save();
                }
            } catch(Exception ex)
            {
                Console.WriteLine("Exception on addHost: " + ex.Message);
            }
        }

        /**
         * Updates webservice api
         * 
         */
        private void sendAPIUpdate(string _data, string _host, string _name)
        {
            string serviceToken = tokenField.Text;

            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["token"] = tokenField.Text;
                values["data"] = _data.Trim();
                values["host"] = _host;
                values["name"] = _name;

                var response = client.UploadValues("http://monitoring.mylifegadgets.com/api/update", "POST", values);

                //var responseString = Encoding.Default.GetString(response);

                //Console.WriteLine(responseString);
            }
        }

        private void monitoringHosts(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            while (this.Monitoring)
            {
                try
                {
                    // Get list from listview
                    if (hostsList.Items.Count > 0)
                    {
                        foreach (ListViewItem hostRow in this.hostsList.Items)
                        {
                            var clientSocket = new System.Net.Sockets.TcpClient();
                            string host = hostRow.SubItems[0].Text;
                            string name = hostRow.SubItems[1].Text;
                            Boolean status = false;

                            //Console.WriteLine("Getting host: " + host); 
                            //clientSocket.Connect(host, 3333);
                            if (clientSocket.ConnectAsync(host, 3333).Wait(1000))
                            {
                                status = true;
                                string get_menu_request = "{\"id\":0,\"jsonrpc\":\"2.0\",\"method\":\"miner_getstat1\"}";
                                NetworkStream serverStream = clientSocket.GetStream();
                                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(get_menu_request);
                                serverStream.Write(outStream, 0, outStream.Length);
                                serverStream.Flush();

                                byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                                serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                                string _returndata = System.Text.Encoding.ASCII.GetString(inStream);

                                EthMonJsonTemplate result = JsonConvert.DeserializeObject<EthMonJsonTemplate>(_returndata);

                                hostRow.SubItems[5].Text = result.result[0]; // Version
                                                                             // ETH Hashrates
                                string[] eth_hashrates = result.result[3].Split(';');
                                string eth_hashrate = "";

                                for (int i = 0; i < eth_hashrates.Length; i++)
                                {
                                    double hashrate = Double.Parse(eth_hashrates[i]) / 1000;

                                    eth_hashrate += "GPU" + i + ": " + hashrate.ToString() + "Mh/s "; // Temps
                                }

                                hostRow.SubItems[2].Text = eth_hashrate; // ETH HR

                                // DCR Hashrates
                                string[] dcr_hashrates = result.result[5].Split(';');
                                string dcr_hashrate = "";
                                if (dcr_hashrates[0] == "off")
                                {
                                    hostRow.SubItems[3].Text = "Mode 1 activated";
                                }
                                else
                                {
                                    for (int i = 0; i < dcr_hashrates.Length; i++)
                                    {
                                        double hashrate = Double.Parse(dcr_hashrates[i]) / 1000;

                                        dcr_hashrate += "GPU" + i + ": " + hashrate.ToString() + "Mh/s "; // Temps
                                    }

                                    hostRow.SubItems[3].Text = dcr_hashrate; // DCR HR
                                }

                                // Temps
                                string[] temp = result.result[6].Split(';');
                                string temps = "";

                                for (int i = 0; i < temp.Length; i++)
                                {
                                    temps += "GPU" + i + ": " + temp[i] + "C (FAN: " + temp[i + 1] + "%) "; // Temps
                                    i++;
                                }

                                hostRow.SubItems[4].Text = temps;

                                // Close socket
                                clientSocket.Close();
                                clientSocket = null;

                                this.hostsList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                                this.hostsList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                                // Update web database for SMS Services
                                sendAPIUpdate(_returndata, host, name);
                            } else
                            {
                                // Update web database for SMS Services
                                sendAPIUpdate("", host, name);

                                // Set values
                                hostRow.SubItems[2].Text = "OFFLINE";
                                hostRow.SubItems[3].Text = "OFFLINE";
                                hostRow.SubItems[4].Text = "OFFLINE";
                                hostRow.SubItems[5].Text = "OFFLINE";
                            }

                            // Print
                            Console.WriteLine("Host: " + host + " updated");
                        }


                    } else
                    {
                        Console.WriteLine("List empty");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("Socket error...");
                }

                // Collect free memory
                GC.Collect();

                // Sleep for next reading
                System.Threading.Thread.Sleep(2000);
            }

            Console.WriteLine("Monitoring stopped..");
        }

        private void startMonitoringMiners()
        {
            if (!this.bw.IsBusy && this.Monitoring == false)
            {
                this.Monitoring = true;
                this.bw.RunWorkerAsync();
                this.startMonitoring.Text = "Stop monitoring";

                // Save token
                settings.accessToken = tokenField.Text;
                settings.Save();
            }
            else
            {
                this.Monitoring = false;
                this.startMonitoring.Text = "Start monitoring";
            }
        }

        private void startMonitoring_Click(object sender, EventArgs e)
        {
            startMonitoringMiners();
        }

        class MySettings : AppSettings<MySettings>
        {
            public Dictionary<string, string> hosts = null;
            public string accessToken = "";
        }

        public class AppSettings<T> where T : new()
        {
            private const string DEFAULT_FILENAME = "settings.json";

            public void Save(string fileName = DEFAULT_FILENAME)
            {
                File.WriteAllText(fileName, JsonConvert.SerializeObject(this));
            }

            public static void Save(T pSettings, string fileName = DEFAULT_FILENAME)
            {
                File.WriteAllText(fileName, JsonConvert.SerializeObject(pSettings));
            }

            public static T Load(string fileName = DEFAULT_FILENAME)
            {
                T t = new T();
                if (File.Exists(fileName))
                    t = (JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName)));
                return t;
            }
        }

        private void removeItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.hostsList.SelectedItems.Count > 0)
                {
                    if (settings.hosts.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> entry in settings.hosts.ToList())
                        {
                            if (this.hostsList.SelectedItems[0].SubItems[0].Text == entry.Value)
                            {
                                settings.hosts.Remove(entry.Key);
                            } 
                        }
                    }

                    this.hostsList.Items.Remove(this.hostsList.SelectedItems[0]);

                    settings.Save();
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void clearList_Click(object sender, EventArgs e)
        {
            if(this.Monitoring)
            {
                startMonitoringMiners();
            }
 
            this.hostsList.Clear();
            settings.hosts.Clear();
            settings.Save();
        }
    }
}
