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
    public class WebClientWithTimeout : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest wr = base.GetWebRequest(address);
            wr.Timeout = 2000; // timeout in milliseconds (ms)
            return wr;
        }
    }

    public partial class Form1 : Form
    {
        private string Version = "0.0.21";
        private string apiVersion = "2.5";

        private BackgroundWorker bw;
        private Boolean Monitoring = false;
        private MySettings settings = MySettings.Load();
        private LogWriter logger = new LogWriter();
        private List<string> bwList = new List<string>();

        public Form1()
        {
            InitializeComponent();
            //Control.CheckForIllegalCrossThreadCalls = false;

            // Load theme
            loadTheme();

            this.bw = new BackgroundWorker();
            this.bw.DoWork += new DoWorkEventHandler(monitoringHosts);

            logger.LogWrite("Ethmonitoring " + this.Version + " starting..");

            this.Text = "EthMonitoring version " + this.Version;

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
                    }
                    else if (entry.type == 7)
                    {
                        newHost.SubItems.Add("SGMiner"); // TYPE
                    }
                    else if (entry.type == 8)
                    {
                        newHost.SubItems.Add("Excavator"); // TYPE
                    }
                    else
                    {
                        newHost.SubItems.Add("CCMiner"); // TYPE
                    }
                    newHost.SubItems.Add("-"); // Last updated
                    newHost.SubItems.Add(entry.password);

                    hostsList.Items.Add(newHost);

                    logger.LogWrite("Adding host: " + entry.host + " With name: " + entry.name);
                }

                if (tokenField.Text.Length > 0)
                {
                    startMonitoringMiners();
                }
            }
        }

        private void loadTheme()
        {
            if(settings.theme == "dark")
            {
                this.BackColor = Color.FromArgb(46,46,46);
                // Labels
                this.label1.ForeColor = Color.FromArgb(255, 255, 255);
                this.label2.ForeColor = Color.FromArgb(255, 255, 255);
                this.label3.ForeColor = Color.FromArgb(255, 255, 255);
                this.label4.ForeColor = Color.FromArgb(255, 255, 255);
                this.label5.ForeColor = Color.FromArgb(255, 255, 255);
                this.label6.ForeColor = Color.FromArgb(255, 255, 255);
                this.label7.ForeColor = Color.FromArgb(255, 255, 255);

                // Boxes
                this.tokenField.BackColor = Color.FromArgb(106, 106, 106);
                this.tokenField.ForeColor = Color.FromArgb(255, 255, 255);

                this.hostField.BackColor = Color.FromArgb(106, 106, 106);
                this.hostField.ForeColor = Color.FromArgb(255, 255, 255);

                this.passwordField.BackColor = Color.FromArgb(106, 106, 106);
                this.passwordField.ForeColor = Color.FromArgb(255, 255, 255);

                this.hostName.BackColor = Color.FromArgb(106, 106, 106);
                this.hostName.ForeColor = Color.FromArgb(255, 255, 255);

                this.minerType.BackColor = Color.FromArgb(106, 106, 106);
                this.minerType.ForeColor = Color.FromArgb(255, 255, 255);

                this.tokenLink.LinkColor = Color.FromArgb(255, 165, 0);

                this.hostsList.BackColor = Color.FromArgb(106, 106, 106);
                this.hostsList.ForeColor = Color.FromArgb(255, 255, 255);
            } else
            {
                this.BackColor = Color.FromArgb(230, 230, 230);
                // Labels
                this.label1.ForeColor = Color.FromArgb(0,0,0);
                this.label2.ForeColor = Color.FromArgb(0, 0, 0);
                this.label3.ForeColor = Color.FromArgb(0, 0, 0);
                this.label4.ForeColor = Color.FromArgb(0, 0, 0);
                this.label5.ForeColor = Color.FromArgb(0, 0, 0);
                this.label6.ForeColor = Color.FromArgb(0, 0, 0);
                this.label7.ForeColor = Color.FromArgb(0, 0, 0);
                this.tokenLink.LinkColor = Color.FromArgb(0, 0, 0);
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
                    newHost.SubItems.Add("-"); // Last updated
                    newHost.SubItems.Add(passwordField.Text); // Password field

                    hostsList.Items.Add(newHost);

                    // New Miner
                    MinersTemplate miner = new MinersTemplate();
                    miner.name = hostName.Text;
                    miner.host = hostField.Text;
                    miner.password = passwordField.Text;
                    if (minerType.Text == "Claymore")
                    {
                        miner.type = 0; // Claymore
                    }
                    else if (minerType.Text == "EWBF")
                    {
                        miner.type = 2; // EWBF
                    }
                    else if (minerType.Text == "SGMiner")
                    {
                        miner.type = 7; // SGMiner
                    }
                    else if (minerType.Text == "Excavator")
                    {
                        miner.type = 8; // Excavator
                    }
                    else
                    {
                        miner.type = 1; // CCminer
                    }

                    settings.hosts.Add(miner);
                    settings.Save();

                    logger.LogWrite("Adding new host: " + hostField.Text + " With name: " + hostName.Text);
                    sendDebugUpdate("Adding new host: " + hostField.Text + " With name: " + hostName.Text);

                    // Clear values
                    hostField.Text = "";
                    hostName.Text = "";
                    passwordField.Text = "";
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Exception on addHost: " + ex.Message);
                logger.LogWrite("Exception on addHost: " + ex.Message);
                sendDebugUpdate("Exception on addHost: " + ex.ToString());
            }
        }

        /**
         * Updates webservice api
         * 
         */
        private void sendAPIUpdate(string _data, string _host, string _name)
        {
            string serviceToken = tokenField.Text;

            using (var client = new WebClientWithTimeout())
            {
                var values = new NameValueCollection();
                values["token"] = tokenField.Text;
                values["data"] = _data.Trim();
                values["host"] = _host;
                values["name"] = _name;

                var response = client.UploadValues("https://monitoring.mylifegadgets.com/api/update", "POST", values);

                //var responseString = Encoding.Default.GetString(response);

                //Console.WriteLine(responseString);
            }
        }

        private void sendAPIUpdate(Stats _stats, string _host, string _name)
        {
            try
            {
                string serviceToken = tokenField.Text;

                using (var client = new WebClientWithTimeout())
                {
                    var values = new NameValueCollection();
                    values["token"] = tokenField.Text;
                    values["data"] = JsonConvert.SerializeObject(_stats);
                    values["host"] = _host;
                    values["name"] = _name;
                    values["version"] = this.apiVersion;

                    var response = client.UploadValues("https://ethmonitoring.com/api/update", "POST", values);

                    //var responseString = Encoding.Default.GetString(response);

                    //Console.WriteLine(responseString);
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Api send exception: " + ex.Message);
            }
        }

        private void sendDebugUpdate(string _debug)
        {
            try
            {
                string serviceToken = tokenField.Text;

                using (var client = new WebClientWithTimeout())
                {
                    var values = new NameValueCollection();
                    values["token"] = tokenField.Text;
                    values["debug"] = _debug;
                    values["version"] = this.apiVersion;

                    var response = client.UploadValues("https://ethmonitoring.com/api/debug", "POST", values);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Api send exception: " + ex.Message);
            }
        }

        private void monitorHost(object sender, DoWorkEventArgs e)
        {
            string[] args = e.Argument as string[];
            int row = int.Parse(args[0]);
            ListViewItem hostRow = GlobalFunctions.getListViewItem(this.hostsList, row);

            string host = hostRow.SubItems[0].Text;
            int port = 0;
            try
            {
                if (host.Contains(":"))
                {
                    string[] hostprm = host.Split(':');
                    host = hostprm[0];
                    port = int.Parse(hostprm[1]);
                }
            } catch(Exception ex)
            {
                Console.WriteLine("Host parsing error: " + host);
            }

            while (this.Monitoring && hostRow != null)
            {
                bool error = false;
                
                string name = hostRow.SubItems[1].Text;
                string type = hostRow.SubItems[6].Text;
                string password = hostRow.SubItems[8].Text;

                try
                {

                    Stats stats;

                    if (type == "Claymore")
                    {
                        if(port == 0)
                        {
                            port = 3333;
                        }
                        // Retrieve EWBF stats
                        DualMinerTemplate miner = new DualMinerTemplate();
                        stats = miner.getStats(host, port, password);
                    }
                    else if (type == "EWBF")
                    {
                        if (port == 0)
                        {
                            port = 42000;
                        }
                        // Retrieve EWBF stats
                        EWBF miner = new EWBF();
                        stats = miner.getStats(host, port);
                    }
                    else if (type == "SGMiner")
                    {
                        if (port == 0)
                        {
                            port = 4028;
                        }
                        // Retrieve SGMiner stats
                        SGMiner miner = new SGMiner();
                        stats = miner.getStats(host, port);
                    }
                    else if (type == "Excavator")
                    {
                        if (port == 0)
                        {
                            port = 3456;
                        }
                        // Retrieve Excavator stats
                        Excavator miner = new Excavator();
                        stats = miner.getStats(host, port);
                    }
                    else
                    {
                        if (port == 0)
                        {
                            port = 4068;
                        }
                        CCMiner miner = new CCMiner();
                        stats = miner.getStats(host, port);
                    }
                    
                    if (stats.online && stats.version.Length > 0)
                    {
                        try
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
                                        double hashrate = 0;
                                        if (Double.TryParse(stats.hashrates[i], out hashrate))
                                        {
                                            hashrate = double.Parse(stats.hashrates[i]) / 1000;
                                        }
                                        eth_hashrate += "GPU" + i + ": " + hashrate.ToString() + "Mh/s "; // Hashrate
                                    }
                                    else if (type == "EWBF")
                                    {
                                        eth_hashrate += "GPU" + i + ": " + stats.hashrates[i] + " Sol/s "; // Hashrate
                                    }
                                    else if (type == "Excavator")
                                    {
                                        eth_hashrate += "GPU" + i + ": " + stats.hashrates[i] + " H/s "; // Hashrate
                                    }
                                    else
                                    {
                                        eth_hashrate += "GPU" + i + ": " + stats.hashrates[i] + "Mh/s "; // Hashrate
                                    }
                                }
                            }

                            GlobalFunctions.listViewEditItem(this.hostsList, row, 2, eth_hashrate); // ETH HR

                            if (type == "Claymore" && stats.dcr_hashrates.Count > 0)
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
                                        double hashrate;
                                        if (stats.dcr_hashrates[i] != "off" && Double.TryParse(stats.dcr_hashrates[i], out hashrate))
                                        {
                                            hashrate = hashrate / 1000;
                                            dcr_hashrate += "GPU" + i + ": " + hashrate.ToString() + "Mh/s "; // Hashrate
                                        }
                                        else
                                        {
                                            dcr_hashrate += "GPU" + i + ": off";
                                        }

                                    }

                                    GlobalFunctions.listViewEditItem(this.hostsList, row, 3, dcr_hashrate); // DCR HR
                                }
                            }
                            else
                            {
                                GlobalFunctions.listViewEditItem(this.hostsList, row, 3, "-");
                            }

                            // Temps
                            string temps = "";

                            for (int i = 0; i < stats.temps.Count; i++)
                            {
                                if (type == "Claymore" || type == "SGMiner" || type == "Excavator")
                                {
                                    if (stats.fan_speeds.Count > 0)
                                    {
                                        temps += "GPU" + i + ": " + stats.temps[i] + "C (FAN: " + stats.fan_speeds[i] + "%) "; // Temps
                                    }
                                    else
                                    {
                                        temps += "GPU" + i + ": " + stats.temps[i] + "C"; // Temps
                                    }
                                }
                                else if (type == "CCMiner")
                                {
                                    if (stats.power_usage.Count > 0 && stats.fan_speeds.Count > 0 && stats.temps.Count > 0)
                                    {
                                        temps += "GPU" + i + ": " + stats.temps[i] + "C (" + stats.power_usage[i] + "W / " + stats.fan_speeds[i] + "%)"; // Temps
                                    }
                                }
                                else
                                {
                                    temps += "GPU" + i + ": " + stats.temps[i] + "C (" + stats.power_usage[i] + "W) "; // Temps
                                }
                                i++;
                            }

                            GlobalFunctions.listViewEditItem(this.hostsList, row, 4, temps);
                            GlobalFunctions.listViewEditItem(this.hostsList, row, 7, DateTime.Now.ToString("HH:mm:ss"));

                            GlobalFunctions.updateColumnSizesForListView(this.hostsList);
                        }
                        catch (Exception ex)
                        {
                            sendDebugUpdate("Miner display exception (type: " + type + "): " +ex.ToString());
                        }
                        // Update web database for SMS Services
                        sendAPIUpdate(stats, host, name);

                    }
                    else
                    {
                        // Update web database for SMS Services
                        sendAPIUpdate(stats, host, name);
                        if (stats.ex != null)
                        {
                            sendDebugUpdate("Host socket exception (Offline | type: " + type + "): " + stats.ex.ToString());
                            GlobalFunctions.listViewEditItem(this.hostsList, row, 2, stats.ex.Message.ToString());
                        } else
                        {
                            GlobalFunctions.listViewEditItem(this.hostsList, row, 2, "OFFLINE");
                        }

                        // Set values
                        GlobalFunctions.listViewEditItem(this.hostsList, row, 3, "OFFLINE");
                        GlobalFunctions.listViewEditItem(this.hostsList, row, 4, "OFFLINE");
                        GlobalFunctions.listViewEditItem(this.hostsList, row, 5, "OFFLINE");
                        GlobalFunctions.listViewEditItem(this.hostsList, row, 7, DateTime.Now.ToString("HH:mm:ss"));
                        error = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                    logger.LogWrite("Host socket exception: " + ex.ToString());
                    sendDebugUpdate("Host socket exception Host: " + host + ":"+port+" Type: " + type + ": " + ex.ToString());

                    // Update web database for SMS Services
                    sendAPIUpdate("", host, name);

                    // Set values
                    GlobalFunctions.listViewEditItem(this.hostsList, row, 2, ex.Message.ToString());
                    GlobalFunctions.listViewEditItem(this.hostsList, row, 3, "OFFLINE");
                    GlobalFunctions.listViewEditItem(this.hostsList, row, 4, "OFFLINE");
                    GlobalFunctions.listViewEditItem(this.hostsList, row, 5, "OFFLINE");
                    GlobalFunctions.listViewEditItem(this.hostsList, row, 7, DateTime.Now.ToString("HH:mm:ss"));
                    error = true;
                }

                if (this.Monitoring)
                {
                    System.Threading.Thread.Sleep(15000);
                }
            }

            // Remove from active worker list
            this.bwList.Remove(host);

            logger.LogWrite("Thread ended for host:" + host);
            sendDebugUpdate("Thread ended for host:" + host);
            Console.WriteLine("Thread ended");
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
                        for(int row = 0;row<GlobalFunctions.listViewCountItems(this.hostsList);row++)
                        {
                            ListViewItem hostRow = GlobalFunctions.getListViewItem(this.hostsList, row);
                            string host = hostRow.SubItems[0].Text;
                            if (!this.bwList.Contains(host))
                            {
                                BackgroundWorker minerWorker = new BackgroundWorker();
                                minerWorker.DoWork += new DoWorkEventHandler(this.monitorHost);
                                minerWorker.RunWorkerAsync(new string[] { row.ToString() });
                                this.bwList.Add(host);
                            }
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
                    sendDebugUpdate("List exception: " + ex.ToString());
                }

                // Collect free memory
                GC.Collect();

                // Sleep for next reading
                System.Threading.Thread.Sleep(2000);
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
                this.removeItem.Enabled = false;
                this.clearList.Enabled = false;
                this.addhost.Enabled = false;

                // Save token
                settings.accessToken = tokenField.Text;
                settings.Save();
            }
            else
            {
                this.Monitoring = false;
                this.startMonitoring.Text = "Start monitoring";
                this.removeItem.Enabled = true;
                this.addhost.Enabled = true;
                this.clearList.Enabled = true;
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
            public string theme = "light";
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

        private void themeButton_Click(object sender, EventArgs e)
        {
            if(settings.theme == "light")
            {
                settings.theme = "dark";
                themeButton.Text = "Light theme";
            } else
            {
                settings.theme = "light";
                themeButton.Text = "Dark theme";
            }

            // Load theme
            loadTheme();

            settings.Save();
        }
    }
}
