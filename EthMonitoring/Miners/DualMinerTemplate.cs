using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EthMonitoring
{

    class EthMonJsonTemplate
    {
        public int id { get; set; }
        public string error { get; set; }
        public List<string> result { get; set; }
    }
    class DualMinerTemplate
    {
        LogWriter logger = new LogWriter();

        public Stats getStats(string _host, int _port, string _password)
        {
            Stats stats = new Stats()
            {
                online = false,
                ex = null,
                uptime = "",
                version = "",
                hashrates = new List<string>(),
                dcr_hashrates = new List<string>(),
                temps = new List<string>(),
                fan_speeds = new List<string>(),
                power_usage = new List<string>(),
                type = 0,
                dual_accepted = 0,
                dual_rejected = 0,
                total_dual_hashrate = ""
            };

            try
            {
                var clientSocket = new System.Net.Sockets.TcpClient();

                if (clientSocket.ConnectAsync(_host, _port).Wait(5000))
                {
                    string get_menu_request = "{\"id\":0,\"jsonrpc\":\"2.0\",\"method\":\"miner_getstat1\",\"psw\":\"" + _password + "\"}";
                    NetworkStream serverStream = clientSocket.GetStream();
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(get_menu_request);
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();

                    byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                    serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                    string _returndata = System.Text.Encoding.ASCII.GetString(inStream);

                    if (_returndata.Length == 0)
                        throw new Exception("Invalid data");

                    Console.WriteLine(_returndata);

                    EthMonJsonTemplate result = JsonConvert.DeserializeObject<EthMonJsonTemplate>(_returndata);

                    stats.version = result.result[0]; // Version
                    stats.uptime = result.result[1]; // Uptime

                    string[] miner_stats = result.result[2].Split(';');
                    stats.total_hashrate = miner_stats[0];
                    stats.accepted = Int32.Parse(miner_stats[1]);
                    stats.rejected = Int32.Parse(miner_stats[2]);

                    // Dual Stats
                    string[] dual_stats = result.result[4].Split(';');
                    stats.total_dual_hashrate = dual_stats[0];
                    stats.dual_accepted = Int32.Parse(dual_stats[1]);
                    stats.dual_rejected = Int32.Parse(dual_stats[2]);

                    string[] hashrates = result.result[3].Split(';'); // ETH Hashrates

                    for(int i = 0; i < hashrates.Length; i++)
                    {
                        stats.hashrates.Add(hashrates[i]);
                    }

                    string[] dcr_hashrates = result.result[5].Split(';'); // DCR Hashrates

                    for (int i = 0; i < dcr_hashrates.Length; i++)
                    {
                        stats.dcr_hashrates.Add(dcr_hashrates[i]);
                    }

                    // Temps and fan speeds
                    string[] temp = result.result[6].Split(';');
                    try
                    {
                        int temp_row = 0;
                        for (int i = 0; i < temp.Length; i++)
                        {
                            stats.temps.Add(temp[i]);
                            stats.fan_speeds.Add(temp[i + 1]);
                            i++;
                            temp_row++;
                        }
                    } catch(Exception ex)
                    {

                    }
                    
                    // Close socket
                    clientSocket.Close();
                    clientSocket = null;

                    stats.online = true; // Online
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                stats.ex = ex;
                logger.LogWrite("Host socket exception: " + ex.ToString());
            }

            return stats;
        }

    }
}
