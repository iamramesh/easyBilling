using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using old = System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Media.Animation;
using easyBilling.Helper;

namespace easyBilling.Startup
{
    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : Window
    {
        long aId = 000;

        static old.Timer myTimer1 = new old.Timer();
        static old.Timer myTimer2 = new old.Timer();
        static old.Timer myTimer3 = new old.Timer();
        static old.Timer myTimer4 = new old.Timer();

        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        public Startup()
        {
            InitializeComponent();

            myTimer1.Tick += new EventHandler(TimerEventProcessor1);
            myTimer2.Tick += new EventHandler(TimerEventProcessor2);
            myTimer3.Tick += new EventHandler(TimerEventProcessor3);
            myTimer4.Tick += new EventHandler(TimerEventProcessor4);

            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);

            this.backgroundWorker.WorkerSupportsCancellation = false;
            this.backgroundWorker.WorkerReportsProgress = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myTimer4.Interval = 4000;
            myTimer4.Start();

            this.txtBuild.Text = "Loading...";
        }

        private void TimerEventProcessor4(Object myObject, EventArgs myEventArgs)
        {
            myTimer4.Stop();

            this.txtBuild.Text = "Build Version : " + Global.AssemblyVersion;

            Storyboard SbShow = (Storyboard)FindResource("Sb_Show");
            SbShow.Begin();
        }

        private void SbShow_Completed(object sender, EventArgs e)
        {
            myTimer1.Interval = 2000;
            myTimer1.Start();
        }

        private void TimerEventProcessor1(Object myObject, EventArgs myEventArgs)
        {
            myTimer1.Stop();
            this.backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DataServices.AppUse au = new DataServices.AppUse();
            au.UserName = Environment.UserName;
            au.MachineName = Environment.MachineName;
            au.StartTime = System.DateTime.Now;

            e.Result = DataServices.DataOperations.AddAppUse(au);
        }

        bool errorFlag = false;

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Convert.ToInt64(e.Result) == 000)
            {
                myTimer3.Interval = 6000;
                myTimer3.Start();

                this.txtBuild.Text = "Error in Starting...";

                this.errorFlag = true;
            }
            else
            {
                aId = Convert.ToInt64(e.Result);

                myTimer2.Interval = 3000;
                myTimer2.Start();
            }
        }

        private void txtBuild_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (errorFlag)
            {
                try
                {
                    System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Rp2-Softwares\\easyBilling");
                }
                catch(Exception ex)
                {
                    LogEntry.WriteEntry("txtBuild_MouseLeftButtonDown", ex.Message);
                }
            }
        }

        private void TimerEventProcessor2(Object myObject, EventArgs myEventArgs)
        {
            MainWindow m = new MainWindow();
            m.Show();

            myTimer2.Stop();
            this.Close();
        }

        private void TimerEventProcessor3(Object myObject, EventArgs myEventArgs)
        {
            Application.Current.Shutdown();
        }
    }
}