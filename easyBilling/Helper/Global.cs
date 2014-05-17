using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace easyBilling.Helper
{
    public class Global
    {
        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
