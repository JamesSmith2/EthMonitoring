using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EthMonitoring
{
    class CCMiner
    {
        private string host;
        private int port;

        private string getCommand(string _cmd)
        {
            string minerData = "";
            var clientSocket = new System.Net.Sockets.TcpClient();

            if (clientSocket.ConnectAsync(this.host, this.port).Wait(5000))
            {
                //string get_menu_request = "threads|";
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(_cmd);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                string _returndata = System.Text.Encoding.ASCII.GetString(inStream);
                minerData = _returndata.Substring(0, _returndata.LastIndexOf("|") + 1);
            }
            else
            {
                Console.WriteLine("CCMiner socket failed");
            }

            // Close socket
            clientSocket.Close();
            clientSocket = null;

            return minerData;
        }

        public Stats getStats(string _host, int _port)
        {
            // Set vars
            this.host = _host;
            this.port = _port;

            // Create stats
            Stats stats = new Stats()
            {
                online = false,
                uptime = "",
                ex = null,
                version = "",
                hashrates = new List<string>(),
                dcr_hashrates = new List<string>(),
                temps = new List<string>(),
                fan_speeds = new List<string>(),
                power_usage = new List<string>(),
                type = 1,
                dual_accepted = 0,
                dual_rejected = 0,
                total_dual_hashrate = ""
            };

            try
            {
                // Fetch summary data
                string minerData = getCommand("summary|");
                if (minerData.Length > 0)
                {
                    string[] data = minerData.Split(';');

                    // Version
                    string name = data[0].Split('=')[1];
                    string version = data[1].Split('=')[1];

                    stats.version = version;
                    stats.total_hashrate = data[5].Split('=')[1];
                    stats.accepted = Int32.Parse(data[7].Split('=')[1]);
                    stats.rejected = Int32.Parse(data[8].Split('=')[1]);

                    // GPU Data
                    string gpuData = getCommand("threads|");

                    string[] gpus = gpuData.Split('|');

                    if (gpus.Length > 0)
                    {
                        for (int i = 0; i < (gpus.Length-1); i++) {
                            string[] gpu = gpus[i].Split(';');
                            double hashrate = 0;
                            int wattage = 0;
                            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                            culture.NumberFormat.NumberDecimalSeparator = ".";
                            float _version = 0;
                            try
                            {
                                _version = float.Parse(version, culture);
                            } catch(Exception ex)
                            {

                            }
                            if (_version >= 2.0)
                            {
                                hashrate = double.Parse(gpu[11].Split('=')[1].Split('.')[0]);
                                wattage = int.Parse(gpu[4].Split('=')[1]) / 1000;
                            }
                            else if(version == "alexis-1.0" || version == "alexis-1.0 palgin skunkmod" || version == "SkunkSPmod1" || version == "SkunkSPmod2" || version == "SkunkSPmod3")
                            {
                                hashrate = double.Parse(gpu[8].Split('=')[1].Split('.')[0]);
                                wattage = int.Parse(gpu[4].Split('=')[1]) / 1000;
                            } else
                            {
                                hashrate = double.Parse(gpu[11].Split('=')[1].Split('.')[0]);
                                wattage = int.Parse(gpu[4].Split('=')[1]) / 1000;
                            }

                            stats.hashrates.Add(hashrate.ToString());
                            stats.temps.Add(gpu[3].Split('=')[1]);
                            stats.power_usage.Add(wattage.ToString());
                            stats.fan_speeds.Add(gpu[5].Split('=')[1]);
                        }
                    }


                    stats.online = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CCMiner Exception: " + ex.StackTrace);
            }

            return stats;
        }
    }
}
