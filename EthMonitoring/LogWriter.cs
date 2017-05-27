using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EthMonitoring
{
    class LogWriter
    {
        private string m_exePath = string.Empty;
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }

        public LogWriter()
        {
        }

        public void LogWrite(string logMessage)
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                // Check file size
                string fileName = m_exePath + "\\" + "log.txt";
                FileInfo txtfile = new FileInfo(fileName);
                if (txtfile.Length > (2 * 1024 * 1024))       // ## NOTE: 2MB max file size
                {
                    var lines = File.ReadAllLines(fileName).Skip(10).ToArray();  // ## Set to 10 lines
                    File.WriteAllLines(fileName, lines);
                }

                using (StreamWriter w = File.AppendText(fileName))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logging exception: " + ex.Message);
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.WriteLine("{0} {1} : {2}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString(), logMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logging exception: " + ex.Message);
            }
        }
    }
}
