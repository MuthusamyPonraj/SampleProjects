using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;
using Microsoft.Web.Administration;
using System.Management;

namespace GetRunTimeException
{
    public class Program
    {        
        static void Main(string[] args)
        {

            //bool status_IIS = IsPortRunning_IIS("57622");
            //bool status_IISExpress = IsPortRunning_IISExpress("57622");




            Process process = new Process();
            process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\cmd.exe";
            process.StartInfo.Verb = "runas";
            process.StartInfo.Arguments = "/C sc delete FNLS-EVERDYN";
            process.StartInfo.RedirectStandardInput = true;            
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.WaitForExit();
            Console.WriteLine(process.StandardOutput.ReadToEnd());

            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "flexnetls");
        }

        public static bool IsPortRunning_IIS(string portNumber)
        {
            int runningPort = 0;
            int currentport = int.Parse(portNumber);
            var server = new ServerManager();
            SiteCollection runningSites = server.Sites;
            foreach (var runningSite in runningSites)
            {
                if (runningSite != null)
                {                    
                        foreach (var binding in runningSite.Bindings)
                        {
                            runningPort = binding.EndPoint.Port;
                        }

                        if (runningPort == currentport)
                        {
                            return true;
                        }                    
                }
            }

            return false;
        }

        public static bool IsPortRunning_IISExpress(string portNumber)
        {
            ManagementClass MgmtClass = new ManagementClass("Win32_Process");
            foreach (ManagementObject mo in MgmtClass.GetInstances())
            {
                if (mo["Name"].ToString().ToLower().Contains("iisexpress"))
                {
                    // Command line contains the config name.
                    //Ex Arg Struture:IISExpress Phyicalpath PortNumber
                    string[] RunningPortDetails = mo["CommandLine"].ToString().Split('/');
                    if (mo["CommandLine"] != null && RunningPortDetails[2].Equals("port:" + portNumber))
                        return true;
                }
            }

            return false;
        }
    }
}
