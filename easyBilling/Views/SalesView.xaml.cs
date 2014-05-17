using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using System.ComponentModel;
using easyBilling.DataServices;
using System.Windows.Threading;
using easyBilling.Helper;

namespace easyBilling.Views
{
    /// <summary>
    /// Interaction logic for SalesView.xaml
    /// </summary>
    public partial class SalesView : UserControl
    {
        private BackgroundWorker bwGetReport = new BackgroundWorker();
        private IniFile ini;

        public SalesView()
        {
            InitializeComponent();

            this.bwGetReport.DoWork +=new DoWorkEventHandler(bwGetReport_DoWork);
            bwGetReport.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwGetReport_RunWorkerCompleted);
            this.bwGetReport.WorkerSupportsCancellation = false;
            this.bwGetReport.WorkerReportsProgress = false;
        }

        private DateTime From;
        private DateTime To;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ini = new IniFile();

            try
            {
                this.rdpFrom.SelectedValue = Convert.ToDateTime(ini.ReadIni()[4]);
                this.rdpTo.SelectedValue = Convert.ToDateTime(ini.ReadIni()[5]);
            }
            catch
            { }
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            this.rdpFrom.GetBindingExpression(RadDatePicker.SelectedValueProperty).UpdateSource();
            this.rdpTo.GetBindingExpression(RadDatePicker.SelectedValueProperty).UpdateSource();

            if (!(Validation.GetHasError(this.rdpFrom) | Validation.GetHasError(this.rdpTo)))
            {
                if (!this.bwGetReport.IsBusy)
                {
                    From = Convert.ToDateTime(this.rdpFrom.SelectedDate);
                    To = Convert.ToDateTime(this.rdpTo.SelectedDate);

                    this.rgSalesReport.IsBusy = true;
                    this.bwGetReport.RunWorkerAsync();
                }
            }
        }

        void bwGetReport_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = DOSalesReport.GetSalesReportByFromTo(From, To);
        }

        void bwGetReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                RadWindow.Alert("Error: " + e.Error.Message);
                
                this.rgSalesReport.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.btnShow.Focus();
                }));
            }
            else
            {
                //------------------- Write Ini Data --------------------------------
                string[] Value_s1 = new string[2];

                Value_s1[0] = Convert.ToDateTime(this.rdpFrom.SelectedValue).ToShortDateString();
                Value_s1[1] = Convert.ToDateTime(this.rdpTo.SelectedValue).ToShortDateString();

                ini = new IniFile();
                ini.WriteIniOnlySalesDate(Value_s1);
                //------------------- Write Ini Data --------------------------------

                this.rgSalesReport.ItemsSource = e.Result;

                this.rgSalesReport.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.rgSalesReport.Focus();
                }));
            }
        }
    }
}
