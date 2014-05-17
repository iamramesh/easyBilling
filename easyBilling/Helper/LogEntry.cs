using System;
using System.Linq;
using System.IO;

namespace easyBilling.Helper
{
    public class LogEntry
    {
        public static void WriteEntry(string f, string m)
        {
            string settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Rp2-Softwares\\easyBilling";

            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }

            string fileName = "easyBilling_Log.log";
            string settingsPath = Path.Combine(settingsDirectory, fileName);

            System.IO.StreamWriter file = new System.IO.StreamWriter(settingsPath, true);

            string s = "[ " + System.DateTime.Now.ToString() + " ]" + "\r\n" + "[ " + Environment.MachineName + " , " + Environment.UserName + " ]" + "\r\n" + "[ " + f + " ]" + "\r\n" + m + "\r\n" + "-------------------------------------------------------------------";
            file.WriteLine(s);
            file.Close();
        }
    }
}
