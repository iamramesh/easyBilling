using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace easyBilling.Helper
{
    public class IniFile
    {
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        static readonly string settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Rp2-Softwares\\easyBilling";
    
        static readonly string fileName = "easyBillingSettings.ini";
        static readonly string filename = Path.Combine(settingsDirectory, fileName);

        static string[] key_s1 = new string[9];

        readonly string section = "GenSection";

        public IniFile()
        {
            key_s1[0] = "StartTime";
            key_s1[1] = "H1";
            key_s1[2] = "H2";
            key_s1[3] = "PrintFlag";

            key_s1[4] = "SalesFrom";
            key_s1[5] = "SalesTo";

            key_s1[6] = "BillFrom";
            key_s1[7] = "BillTo";
            key_s1[8] = "BillDate";
        }

        public void WriteIni(string[] Value_s1)
        {
            try
            {
                WritePrivateProfileString(section, key_s1[0], Value_s1[0], filename);
                WritePrivateProfileString(section, key_s1[1], Value_s1[1], filename);
                WritePrivateProfileString(section, key_s1[2], Value_s1[2], filename);
                WritePrivateProfileString(section, key_s1[3], Value_s1[3], filename);

                WritePrivateProfileString(section, key_s1[4], Value_s1[4], filename);
                WritePrivateProfileString(section, key_s1[5], Value_s1[5], filename);
                WritePrivateProfileString(section, key_s1[6], Value_s1[6], filename);
                WritePrivateProfileString(section, key_s1[7], Value_s1[7], filename);
                WritePrivateProfileString(section, key_s1[8], Value_s1[8], filename);
            }
            catch (Exception e)
            {
                Helper.LogEntry.WriteEntry("WriteIni", e.Message);
            }
        }

        public void WriteIniOnlySalesDate(string[] Value_s1)
        {
            try
            {
                WritePrivateProfileString(section, key_s1[4], Value_s1[0], filename);
                WritePrivateProfileString(section, key_s1[5], Value_s1[1], filename);
            }
            catch (Exception e)
            {
                LogEntry.WriteEntry("WriteIniOnlySalesDate", e.Message);
            }
        }

        public void WriteIniOnlyBillPrint(string[] Value_s1)
        {
            try
            {
                WritePrivateProfileString(section, key_s1[6], Value_s1[0], filename);
                WritePrivateProfileString(section, key_s1[7], Value_s1[1], filename);
                WritePrivateProfileString(section, key_s1[8], Value_s1[2], filename);
            }
            catch (Exception e)
            {
                LogEntry.WriteEntry("WriteIniOnlyBillPrint", e.Message);
            }
        }

        public string[] ReadIni()
        {
            try
            {
                string[] temp = new string[9];

                int chars = 256;
                StringBuilder buffer = new StringBuilder(chars);

                string sDefault = "";

                GetPrivateProfileString(section, key_s1[0], sDefault, buffer, chars, filename);
                temp[0] = buffer.ToString();

                GetPrivateProfileString(section, key_s1[1], sDefault, buffer, chars, filename);
                temp[1] = buffer.ToString();

                GetPrivateProfileString(section, key_s1[2], sDefault, buffer, chars, filename);
                temp[2] = buffer.ToString();

                GetPrivateProfileString(section, key_s1[3], sDefault, buffer, chars, filename);
                temp[3] = buffer.ToString();

                GetPrivateProfileString(section, key_s1[4], sDefault, buffer, chars, filename);
                temp[4] = buffer.ToString();

                GetPrivateProfileString(section, key_s1[5], sDefault, buffer, chars, filename);
                temp[5] = buffer.ToString();

                GetPrivateProfileString(section, key_s1[6], sDefault, buffer, chars, filename);
                temp[6] = buffer.ToString();

                GetPrivateProfileString(section, key_s1[7], sDefault, buffer, chars, filename);
                temp[7] = buffer.ToString();

                GetPrivateProfileString(section, key_s1[8], sDefault, buffer, chars, filename);
                temp[8] = buffer.ToString();

                return temp;
            }
            catch (Exception e)
            {
                Helper.LogEntry.WriteEntry("ReadIni", e.Message);
                return null;
            }
        }
    }
}
