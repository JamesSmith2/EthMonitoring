using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EthMonitoring
{
    class GPUTemplate
    {
        public int gpu_temp { get; set; }
        public int gpu_fan_speed { get; set; }
        public string name { get; set; }
    }

    class DeviceTemplate
    {
        public int device_id { get; set; }
    }

    class DeviceListTemplate
    {
        public List<DeviceTemplate> devices { get; set; }
    }

    class ExcavatorInfoTemplate
    {
        public string version { get; set; }
        public string error { get; set; }
        public string uptime { get; set; }
    }

    class AlgorithmWorkerTemplate
    {
        public int worker_id { get; set; }
        public int device_id { get; set; }
        public double speed { get; set; }
    }

    class AlgorithmDetailsTemplate
    {
        public int total_accepted { get; set; }
        public int total_rejected { get; set; }
    }

    class AlgorithmTemplate
    {
        public int algorithm_id { get; set; }
        public AlgorithmDetailsTemplate details { get; set; }
        public List<AlgorithmWorkerTemplate> workers { get; set; }
    }

    class AlgorithmsTemplate
    {
        public List<AlgorithmTemplate> algorithms { get; set; }
    }

    class Excavator
    {
        private string host;
        private int port;
        private LogWriter logger = new LogWriter();

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
                minerData = _returndata.Substring(0, _returndata.LastIndexOf("}") + 1);
            }
            else
            {
                Console.WriteLine("Excavator socket failed");
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
                type = 8,
                dual_accepted = 0,
                dual_rejected = 0,
                total_dual_hashrate = ""
            };

            try
            {
                // Fetch summary data
                string minerData = getCommand("{\"id\":1,\"method\":\"info\",\"params\":[]}\n");

                ExcavatorInfoTemplate result = JsonConvert.DeserializeObject<ExcavatorInfoTemplate>(minerData);

                if (result.version.Length > 0)
                {
                    // Version
                    stats.version = result.version;
                    stats.uptime = result.uptime;

                    // GPU Data
                    string gpuData = getCommand("{\"id\":1,\"method\":\"device.list\",\"params\":[]}\n");
                    
                    DeviceListTemplate devices = JsonConvert.DeserializeObject<DeviceListTemplate>(gpuData);

                    if (devices.devices.Count > 0)
                    {
                        for (int i = 0; i < devices.devices.Count; i++)
                        {
                            int device_id = devices.devices[i].device_id;

                            string gpuInfoData = getCommand("{\"id\":1,\"method\":\"device.get\",\"params\":[\"" + device_id.ToString() + "\"]}\n");

                            GPUTemplate gpu = JsonConvert.DeserializeObject<GPUTemplate>(gpuInfoData);

                            stats.hashrates.Add("0");
                            stats.temps.Add(gpu.gpu_temp.ToString());
                            stats.power_usage.Add("0");
                            stats.fan_speeds.Add(gpu.gpu_fan_speed.ToString());
                        }
                    }

                    // Algorithm
                    string algorithmData = getCommand("{\"id\":1,\"method\":\"algorithm.list\",\"params\":[]}\n");

                    AlgorithmsTemplate algorithms = JsonConvert.DeserializeObject<AlgorithmsTemplate>(algorithmData);

                    double total_speed = 0;
                    List<double> speeds = new List<double>();
                    if(algorithms.algorithms.Count > 0)
                    {
                        for (int i = 0; i < algorithms.algorithms.Count; i++)
                        {
                            AlgorithmTemplate algorithm = algorithms.algorithms[i];
                            stats.accepted = algorithm.details.total_accepted;
                            stats.rejected = algorithm.details.total_rejected;

                            // Workers
                            if(algorithm.workers.Count > 0)
                            {
                                for(int w = 0; w < algorithm.workers.Count; w++)
                                {
                                    AlgorithmWorkerTemplate worker = algorithm.workers[w];
                                    string speed = stats.hashrates[worker.device_id];
                                    Double hashrate = Double.Parse(speed);
                                    double WorkerSpeed = Math.Round(worker.speed, 2);
                                    hashrate += WorkerSpeed;
                                    total_speed += WorkerSpeed;
                                    stats.hashrates[worker.device_id] = hashrate.ToString();
                                }
                            }
                        }
                    }

                    stats.total_hashrate = total_speed.ToString();
                    stats.online = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excavator Exception: " + ex.StackTrace);
            }

            return stats;
        }
    }
}
