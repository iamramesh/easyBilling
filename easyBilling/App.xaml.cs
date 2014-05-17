using System;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using System.Windows.Media;

namespace easyBilling
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MetroColors.PaletteInstance.AccentColor = Color.FromArgb(0xFF, 0x79, 0x25, 0x6B);           

            StyleManager.ApplicationTheme = new MetroTheme();

            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-IN");
        }
    }
}
