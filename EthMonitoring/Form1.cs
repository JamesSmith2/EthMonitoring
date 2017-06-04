using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private LogWriter logger = new LogWriter();

        public Form1()
        {
            InitializeComponent();
            //Control.CheckForIllegalCrossThreadCalls = false;

            this.bw = new BackgroundWorker();
            this.bw.DoWork += new DoWorkEventHandler(monitoringHosts);

            logger.LogWrite("Ethmonitoring v0.0.7 starting..");

            // Generate dictonary if needed
            if (settings.hosts == null)
            {
                settings.hosts = new List<MinersTemplate>();
            }

            // Set token value
            tokenField.Text = settings.accessToken;

            if (settings.hosts != null && settings.hosts.Count > 0)
            {
                foreach (MinersTemplate entry in settings.hosts)
                {
                    ListViewItem newHost = new ListViewItem(entry.host);

                    newHost.SubItems.Add(entry.name); // Name
                    newHost.SubItems.Add("Initializing.."); // ETH HASHRATE
                    newHost.SubItems.Add("Initializing.."); // DCR HASHRATE
                    newHost.SubItems.Add("Initializing.."); // TEMPERATURES
                    newHost.SubItems.Add(""); // VERSION
                    if (entry.type == 0)
                    {
                        newHost.SubItems.Add("Claymore"); // TYPE
                    }
                    else if (entry.type == 2)
                    {
                        newHost.SubItems.Add("EWBF"); // TYPE
                    } else
                    {
                        newHost.SubItems.Add("CCMiner"); // TYPE
                    }

                    hostsList.Items.Add(newHost);

                    logger.LogWrite("Adding host: " + entry.host + " With name: " + entry.name);
                }

                if (tokenField.Text.Length > 0)
                {
                    startMonitoringMiners();
                }
            }
        }

        private void addhost_Click(object sender, EventArgs e)
        {
            try
            {
                if (hostField.Text != "" && hostName.Text != "" && minerType.Text != "")
                {
                    ListViewItem newHost = new ListViewItem(hostField.Text);

                    newHost.SubItems.Add(hostName.Text); // Hostname
                    newHost.SubItems.Add("Initializing.."); // ETH HASHRATE
                    newHost.SubItems.Add("Initializing.."); // DCR HASHRATE
                    newHost.SubItems.Add("Initializing.."); // TEMPERATURES
                    newHost.SubItems.Add(""); // VERSION
                    newHost.SubItems.Add(minerType.Text); // Type

                    hostsList.Items.Add(newHost);

                    // New Miner
                    MinersTemplate miner = new MinersTemplate();
                    miner.name = hostName.Text;
                    miner.host = hostField.Text;
                    if (minerType.Text == "Claymore")
                    {
                        miner.type = 0; // Claymore
                    }
                    else if (minerType.Text == "EWBF")
                    {
                        miner.type = 2; // EWBF
                    }
                    else
                    {
                        miner.type = 1; // CCminer
                    }

                    settings.hosts.Add(miner);
                    settings.Save();

                    logger.LogWrite("Adding new host: " + hostField.Text + " With name: " + hostName.Text);

                    // Clear values
                    hostField.Text = "";
                    hostName.Text = "";
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Exception on addHost: " + ex.Message);
                logger.LogWrite("Exception on addHost: " + ex.Message);
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

        private void sendAPIUpdate(Stats _stats, string _host, string _name)
        {
            try
            {
                string serviceToken = tokenField.Text;

                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["token"] = tokenField.Text;
                    values["data"] = JsonConvert.SerializeObject(_stats);
                    values["host"] = _host;
                    values["name"] = _name;
                    values["version"] = "1.1";

                    var response = client.UploadValues("http://monitoring.mylifegadgets.com/api/update", "POST", values);

                    //var responseString = Encoding.Default.GetString(response);

                    //Console.WriteLine(responseString);
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Api send exception: " + ex.Message);
            }
        }

        private void monitoringHosts(object sender, System.ComponentModel.DoWorkEventArgs e)
        {


            logger.LogWrite("Starting monitoring...");

            while (this.Monitoring)
            {
                try
                {
                    // Get list from listview
                    if (GlobalFunctions.listViewCountItems(this.hostsList) > 0)
                    {
                        for(int row = 0;row< GlobalFunctions.listViewCountItems(this.hostsList);row++)
                        {

                            ListViewItem hostRow = GlobalFunctions.getListViewItem(this.hostsList, row);

                            string host = hostRow.SubItems[0].Text;
                            string name = hostRow.SubItems[1].Text;
                            string type = hostRow.SubItems[6].Text;

                            try {

                                Stats stats;
                                
                                if (type == "Claymore")
                                {
                                    // Retrieve EWBF stats
                                    DualMinerTemplate miner = new DualMinerTemplate();
                                    stats = miner.getStats(host, 3333);
                                }
                                else if (type == "EWBF")
                                {
                                    // Retrieve EWBF stats
                                    EWBF miner = new EWBF();
                                    stats = miner.getStats(host, 42000);
                                } else
                                {
                                    CCMiner miner = new CCMiner();
                                    stats = miner.getStats(host, 4068);
                                }

                                if (stats.online)
                                {
                                    GlobalFunctions.listViewEditItem(this.hostsList, row, 5, stats.version); // Version

                                    // ETH Hashrates
                                    string eth_hashrate = "";
                                    if (stats.hashrates.Count > 0 && stats.hashrates[0] == "off")
                                    {
                                        GlobalFunctions.listViewEditItem(this.hostsList, row, 2, "Mode 2 activated");
                                    }
                                    else
                                    {
                                        for (int i = 0; i < stats.hashrates.Count; i++)
                                        {
                                            if (type == "Claymore" || type == "CCMiner")
                                            {
                                                double hashrate = double.Parse(stats.hashrates[i]) / 1000;
                                                eth_hashrate += "GPU" + i + ": " + hashrate.ToString() + "Mh/s "; // Hashrate
                                            } else if(type == "EWBF")
                                            {
                                                eth_hashrate += "GPU" + i + ": " + stats.hashrates[i] + " Sol/s "; // Hashrate
                                            } else
                                            {
                                                eth_hashrate += "GPU" + i + ": " + stats.hashrates[i] + "Mh/s "; // Hashrate
                                            }
                                        }
                                    }
                                    
                                    GlobalFunctions.listViewEditItem(this.hostsList, row, 2, eth_hashrate); // ETH HR

                                    if (type == "Claymore")
                                    {
                                        // DCR Hashrates
                                        string dcr_hashrate = "";
                                        if (stats.dcr_hashrates[0] == "off")
                                        {
                                            GlobalFunctions.listViewEditItem(this.hostsList, row, 3, "Mode 1 activated");
                                        }
                                        else
                                        {
                                            for (int i = 0; i < stats.dcr_hashrates.Count; i++)
                                            {
                                                    double hashrate = Double.Parse(stats.dcr_hashrates[i]) / 1000;
                                                    dcr_hashrate += "GPU" + i + ": " + hashrate.ToString() + "Mh/s "; // Hashrate
                                                
                                            }
                                            
                                            GlobalFunctions.listViewEditItem(this.hostsList, row, 3, dcr_hashrate); // DCR HR
                                        }
                                    } else
                                    {
                                        GlobalFunctions.listViewEditItem(this.hostsList, row, 3, "-");
                                    }

                                    // Temps
                                    string temps = "";

                                    for (int i = 0; i < stats.temps.Count; i++)
                                    {
                                        if (type == "Claymore")
                                        {
                                            temps += "GPU" + i + ": " + stats.temps[i] + "C (FAN: " + stats.fan_speeds[i] + "%) "; // Temps
                                        }
                                        else if(type == "CCMiner")
                                        {
                                            temps += "GPU" + i + ": " + stats.temps[i] + "C (" + stats.power_usage[i] + "W / " + stats.fan_speeds[i] + "%)"; // Temps
                                        }else
                                        {
                                            temps += "GPU" + i + ": " + stats.temps[i] + "C (" + stats.power_usage[i] + "W) "; // Temps
                                        }
                                        i++;
                                    }
                                    
                                    GlobalFunctions.listViewEditItem(this.hostsList, row, 4, temps);

                                    GlobalFunctions.updateColumnSizesForListView(this.hostsList);

                                    // Update web database for SMS Services
                                    sendAPIUpdate(stats, host, name);

                                } else
                                {
                                    // Update web database for SMS Services
                                    sendAPIUpdate(stats, host, name);

                                    // Set values
                                    GlobalFunctions.listViewEditItem(this.hostsList, row, 2, "OFFLINE");
                                    GlobalFunctions.listViewEditItem(this.hostsList, row, 3, "OFFLINE");
                                    GlobalFunctions.listViewEditItem(this.hostsList, row, 4, "OFFLINE");
                                    GlobalFunctions.listViewEditItem(this.hostsList, row, 5, "OFFLINE");
                                    
                                    logger.LogWrite("Host not responsive: " + host);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);

                                logger.LogWrite("Host socket exception: " + ex.Message);

                                // Update web database for SMS Services
                                sendAPIUpdate("", host, name);

                                // Set values
                                GlobalFunctions.listViewEditItem(this.hostsList, row, 2, "OFFLINE");
                                GlobalFunctions.listViewEditItem(this.hostsList, row, 3, "OFFLINE");
                                GlobalFunctions.listViewEditItem(this.hostsList, row, 4, "OFFLINE");
                                GlobalFunctions.listViewEditItem(this.hostsList, row, 5, "OFFLINE");
                            }

                            // Print
                            //Console.WriteLine("Host: " + host + " updated");
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
                    logger.LogWrite("List exception: " + ex.Message);
                }

                // Collect free memory
                GC.Collect();

                // Sleep for next reading
                System.Threading.Thread.Sleep(10000);
            }


            logger.LogWrite("Monitoring stopped...");

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
            public List<MinersTemplate> hosts = null;
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
                try
                {
                    if (File.Exists(fileName))
                        t = (JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName)));
                } catch(Exception ex)
                {
                    Console.WriteLine("Invalid json: " + ex.Message);
                }
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
                        foreach (MinersTemplate entry in settings.hosts.ToList())
                        {
                            if (this.hostsList.SelectedItems[0].SubItems[0].Text == entry.host)
                            {
                                settings.hosts.Remove(entry);
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

        private void tokenLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("http://monitoring.mylifegadgets.com");
            Process.Start(sInfo);
        }
    }
}
