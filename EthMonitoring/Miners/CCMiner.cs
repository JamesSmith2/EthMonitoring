using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EthMonitoring
{
    class CCMiner
    {

        public Stats getStats(string _host, int _port)
        {
            Stats stats = new Stats()
            {
                online = false,
                hashrates = new List<string>(),
                dcr_hashrates = new List<string>(),
                temps = new List<string>(),
                fan_speeds = new List<string>(),
                power_usage = new List<string>(),
                type = 1
            };

            try
            {
                var clientSocket = new System.Net.Sockets.TcpClient();

                if (clientSocket.ConnectAsync(_host, _port).Wait(1000))
                {
                    string get_menu_request = "hwinfo|";
                    NetworkStream serverStream = clientSocket.GetStream();
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(get_menu_request);
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();

                    byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                    serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                    string _returndata = System.Text.Encoding.ASCII.GetString(inStream);

                    Console.WriteLine("DATA: " + _returndata);

                    string[] data = _returndata.Split(';');


                    // Close socket
                    clientSocket.Close();
                    clientSocket = null;

                    stats.online = true; // Online
                } else
                {
                    Console.WriteLine("CCMiner socket failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CCMiner Exception: " + ex.Message);
            }

            return stats;
        }
    }
}
