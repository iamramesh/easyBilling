using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using easyBilling.Helper;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.IO;
using System.Windows.Threading;

namespace easyBilling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        long aId = 000;
        MainViewModel mTab;
        TabViewModel tEntry, tItem, tCust, tSales, tHome;

        private ViewControls.PrintBillView pbv;
        //private ViewControl.FinalBillView fbv;
        //private ViewControl.ReloadChargesView rc;
        //private ViewControl.OptionView ov;
        private Helper.IniFile ini;

        private DispatcherTimer tm1;
        private DispatcherTimer tm2;
        private DispatcherTimer tm3;

        private BackgroundWorker bwClose = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            this.bwClose.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwClose_DoWork);
            this.bwClose.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwClose_RunWorkerCompleted);

            this.bwClose.WorkerSupportsCancellation = false;
            this.bwClose.WorkerReportsProgress = false;

            tm1 = new DispatcherTimer();
            tm1.Tick += new EventHandler(tm1_Tick);

            tm2 = new DispatcherTimer();
            tm2.Tick += new EventHandler(tm2_Tick);

            tm3 = new DispatcherTimer();
            tm3.Tick += new EventHandler(tm3_Tick);

            //mTab = new MainViewModel();

            //tEntry = new TabViewModel(mTab);
            //tEntry.Header = "Bill Entry";
            //tEntry.ImgUrl = "Images/home.png";
            //tEntry.UIControl = new Views.BillEntryView(this);
            //tEntry.IsSelected = true;

            //mTab.AddItem(tEntry);

            //this.DataContext = mTab;

            this.txtPass.Password = "admin";

            mTab = new MainViewModel();

            tHome = new TabViewModel(mTab);
            tHome.Header = "Home Page";
            tHome.ImgUrl = "Images/home.png";
            tHome.UIControl = new Views.HomeView();
            tHome.IsSelected = true;

            mTab.AddItem(tHome);

            this.DataContext = mTab;
        }

        private void bwClose_DoWork(object sender, DoWorkEventArgs e)
        {
            //DataServices.AppUse au = new DataServices.AppUse();

            //au.aId = this.aId;
            //au.EndTime = System.DateTime.Now;

            //e.Result = DataServices.DataOperations.UpdateAppUse(au);
        }

        private void bwClose_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RadMenu_ItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadMenuItem item = e.OriginalSource as RadMenuItem;

            if (item != null)
            {
                switch (item.Header.ToString())
                {
                    case "Bill Entry":
                        {
                            if (!mTab.Tabs.Contains(tEntry))
                            {
                                tEntry = new TabViewModel(mTab);
                                tEntry.Header = "Bill Entry";
                                tEntry.ImgUrl = "Images/pencil.png";
                                tEntry.UIControl = new Views.BillEntryView(this);
                                tEntry.IsSelected = true;

                                mTab.AddItem(tEntry);
                            }
                            else
                            {
                                tEntry.IsSelected = true;
                            }

                            break;
                        }

                    case "Item Entry":
                        {
                            if (!mTab.Tabs.Contains(tItem))
                            {
                                tItem = new TabViewModel(mTab);
                                tItem.Header = "Item Entry";
                                tItem.ImgUrl = "Images/page.png";
                                tItem.UIControl = new Views.StockItemView(this);
                                tItem.IsSelected = true;

                                mTab.AddItem(tItem);
                            }
                            else
                            {
                                tItem.IsSelected = true;
                            }

                            break;
                        }

                    case "Customer Entry":
                        {
                            if (!mTab.Tabs.Contains(tCust))
                            {
                                tCust = new TabViewModel(mTab);
                                tCust.Header = "Customer Entry";
                                tCust.ImgUrl = "Images/page.png";
                                tCust.UIControl = new Views.CustomerView(this);
                                tCust.IsSelected = true;

                                mTab.AddItem(tCust);
                            }
                            else
                            {
                                tCust.IsSelected = true;
                            }

                            break;
                        }

                    case "Sales Report":
                        {
                            if (!mTab.Tabs.Contains(tSales))
                            {
                                tSales = new TabViewModel(mTab);
                                tSales.Header = "Sales Report";
                                tSales.ImgUrl = "Images/chart.png";
                                tSales.UIControl = new Views.SalesView();
                                tSales.IsSelected = true;

                                mTab.AddItem(tSales);
                            }
                            else
                            {
                                tSales.IsSelected = true;
                            }

                            break;
                        }

                    case "Print Bill":
                        {
                            pbv = new ViewControls.PrintBillView();
                            if (this.pbv.IsOpen == false)
                            {
                                this.pbv.Owner = this;
                                this.pbv.ShowDialog();
                            }
                            else
                            {
                                this.pbv.BringToFront();
                            }

                            break;
                        }

                    case "Homepage":
                        {
                            //if (!mTab.Tabs.Contains(tHome))
                            //{
                            //    tHome = new TabViewModel(mTab);
                            //    tHome.Header = "Home Page";
                            //    tHome.UIControl = new Views.HomeView();
                            //    tHome.IsSelected = true;

                            //    mTab.AddItem(tHome);
                            //}
                            //else
                            //{
                            //    tHome.IsSelected = true;
                            //}

                            break;
                        }

                    case "Get Started":
                        {
                            break;
                        }

                    case "Exit":
                        {
                            Application.Current.Shutdown();

                            break;
                        }

                    case "Settings":
                        {
                            //ov = new ViewControl.OptionView();
                            //if (this.ov.IsOpen == false)
                            //{
                            //    this.ov.Owner = this;
                            //    this.ov.ShowDialog();
                            //}
                            //else
                            //{
                            //    this.ov.BringToFront();
                            //}

                            break;
                        }

                    case "About easyBilling":
                        {
                            this.bdAbout.Visibility = Visibility.Visible;

                            break;
                        }
                }
            }
        }

        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    e.Cancel = true;
        //    this.Visibility = Visibility.Collapsed;

        //    this.backgroundWorker.RunWorkerAsync();
        //}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtAbtVersion.Text = Global.AssemblyVersion;
            this.txtBottomVersion.Text = Global.AssemblyVersion;

            this.dLogin();
            tm1.Interval = TimeSpan.FromSeconds(600);

            tm2.Interval = TimeSpan.FromSeconds(1);
            tm2.Start();

            string settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Rp2-Softwares\\easyBilling";

            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }

            string fileName = "easyBillingSettings.ini";
            string settingPath = System.IO.Path.Combine(settingsDirectory, fileName);

            FileInfo settingFile = new FileInfo(settingPath);

            if (!settingFile.Exists)
            {
                string[] Value_s1 = new string[9];

                Value_s1[0] = Helper.Crypt.EncryptString("admin", "me");
                Value_s1[1] = "4.4";
                Value_s1[2] = "0.22";
                Value_s1[3] = "1";

                Value_s1[4] = System.DateTime.Now.ToShortDateString();
                Value_s1[5] = System.DateTime.Now.ToShortDateString();
                Value_s1[6] = System.DateTime.Now.ToShortDateString();
                Value_s1[7] = System.DateTime.Now.ToShortDateString();
                Value_s1[8] = System.DateTime.Now.ToShortDateString();

                ini = new IniFile();
                ini.WriteIni(Value_s1);
            }
        }

        private void bdAbout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.bdAbout.Visibility = Visibility.Collapsed;
        }

        private void bdAbout_KeyDown(object sender, KeyEventArgs e)
        {
            this.bdAbout.Visibility = Visibility.Collapsed;
        }

        public static RoutedCommand cmdTab = new RoutedCommand();

        public void CanExecuteCmdTab(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void ExecuteCmdTab(object sender, ExecutedRoutedEventArgs e)
        {
            int s = this.tabControl.SelectedIndex;

            if (s != this.tabControl.Items.Count - 1)
            {
                s++;
                this.tabControl.SelectedIndex = s;
            }
            else
            {
                this.tabControl.SelectedIndex = 0;
            }
        }

        //Start ---- Login Operation -------------------------------------------------------

        private void setDateNtime()
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            DateTime now = DateTime.Now;

            string st = now.ToString("T", ci);
            string sd = now.ToString("dddd, d MMMM", ci);

            this.txtDate.Text = sd;
            this.txtTime.Text = st;
        }

        private void dLogin()
        {
            this.setDateNtime();

            this.txtUser.Text = Environment.UserName;
            this.bdLogin.Visibility = Visibility.Visible;

            this.radMenu.IsEnabled = false;
            this.tabControl.IsEnabled = false;

            tm3.Interval = TimeSpan.FromSeconds(1.5);
            tm3.Start();
        }

        void tm1_Tick(object sender, EventArgs e)
        {
            this.dLogin();
        }

        void tm2_Tick(object sender, EventArgs e)
        {
            this.setDateNtime();
        }

        void tm3_Tick(object sender, EventArgs e)
        {
            (this.Resources["sb_UnLoad"] as Storyboard).Begin();
        }

        private void eLogin()
        {
            (this.Resources["sb_Exit"] as Storyboard).Begin();
            (this.Resources["sb_ImgExit"] as Storyboard).Begin();
        }

        private void sb_UnLoad_Completed(object sender, EventArgs e)
        {
            tm3.Stop();
            this.gridLoading.Visibility = Visibility.Collapsed;

            (this.Resources["sb_Enter"] as Storyboard).Begin();
        }

        private void sb_Enter_Completed(object sender, EventArgs e)
        {
            this.txtPass.Focus();

            (this.Resources["sb_ImgEnter"] as Storyboard).Begin();
        }

        private void sb_Exit_Completed(object sender, EventArgs e)
        {
            this.bdLogin.Visibility = Visibility.Collapsed;

            this.radMenu.IsEnabled = true;
            this.tabControl.IsEnabled = true;
        }

        private void tabControl_KeyDown(object sender, KeyEventArgs e)
        {
            tm1.Stop();
            tm1.Start();
        }

        private void tabControl_MouseMove(object sender, MouseEventArgs e)
        {
            tm1.Stop();
            tm1.Start();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            ini = new IniFile();

            if (this.txtPass.Password == Helper.Crypt.DecryptString(ini.ReadIni()[0].ToString(), "me"))
            {
                this.eLogin();
                tm1.Start();

                this.txtPass.Password = "";
            }
            else
            {
                this.txtPass.Password = "";
                this.txtPass.Focus();
            }
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ini = new IniFile();

                if (this.txtPass.Password == Helper.Crypt.DecryptString(ini.ReadIni()[0].ToString(), "me"))
                {
                    this.eLogin();
                    tm1.Start();

                    this.txtPass.Password = "";
                }
                else
                {
                    this.txtPass.Password = "";
                    this.txtPass.Focus();
                }
            }
        }

        private void txtLogOut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.dLogin();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
        //End ---- Login Operation -------------------------------------------------------
    }
}
