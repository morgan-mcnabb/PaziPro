using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaziPro
{
    public class WifiService
    {
        public async Task<bool> ConnectToWiFi(string ssid, string password)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"nmcli dev wifi connect '{ssid}' password '{password}'\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string result = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Error connecting to WiFi: " + error);
                    return false;
                }

                Console.WriteLine("WiFi connected: " + result);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while connecting to WiFi: " + ex.Message);
                return false;
            }
        }

    }
}
