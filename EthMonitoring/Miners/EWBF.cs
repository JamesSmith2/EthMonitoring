using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EthMonitoring
{
    class EWBFOBjectTemplate
    {
        public int temperature { get; set; }
        public int speed_sps { get; set; }
        public int accepted_shares { get; set; }
        public int rejected_shares { get; set; }
        public int gpu_power_usage { get; set; }
    }

    class EWBFTemplate
    {
        public int id { get; set; }
        public List<EWBFOBjectTemplate> result { get; set; }
    }

    class EWBF
    {
        public Stats getStats(string _host, int _port)
        {
            Stats stats = new Stats()
            {
                online = false,
                uptime = "",
                hashrates = new List<string>(),
                dcr_hashrates = new List<string>(),
                temps = new List<string>(),
                fan_speeds = new List<string>(),
                power_usage = new List<string>(),
                type = 2
            };
            
            try
            {
                var clientSocket = new System.Net.Sockets.TcpClient();

                if (clientSocket.ConnectAsync(_host, _port).Wait(5000))
                {
                    string get_menu_request = "{\"id\":1, \"method\":\"getstat\"}\n";
                    NetworkStream serverStream = clientSocket.GetStream();
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(get_menu_request);
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();

                    byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                    serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                    string _returndata = System.Text.Encoding.ASCII.GetString(inStream);
                    string jsonData = _returndata.Substring(0, _returndata.LastIndexOf("}")+1);

                    EWBFTemplate result = JsonConvert.DeserializeObject<EWBFTemplate>(jsonData);

                    stats.version = "EWBF";

                    int total_hashrate = 0;

                    if (result.result.Count > 0)
                    {
                        foreach (EWBFOBjectTemplate gpu in result.result)
                        {
                            // Speed
                            stats.hashrates.Add(gpu.speed_sps.ToString());
                            stats.power_usage.Add(gpu.gpu_power_usage.ToString());
                            stats.fan_speeds.Add("0");
                            stats.temps.Add(gpu.temperature.ToString());
                            total_hashrate += gpu.speed_sps;

                            // Shares
                            stats.accepted += gpu.accepted_shares;
                            stats.rejected += gpu.rejected_shares;

                        }
                    }

                    stats.total_hashrate = total_hashrate.ToString();

                    // Close socket
                    clientSocket.Close();
                    clientSocket = null;

                    stats.online = true; // Online
                }
                else
                {
                    Console.WriteLine("EWBF socket failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EWBF Exception: " + ex.Message);
            }

            return stats;
        }
    }
}
