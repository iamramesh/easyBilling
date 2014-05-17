using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using easyBilling.Helper;
using System.Windows.Threading;
using easyBilling.DataServices;

namespace easyBilling.ViewControls
{
    /// <summary>
    /// Interaction logic for PrintBillView.xaml
    /// </summary>
    public partial class PrintBillView : RadWindow
    {
        private bool _loaded, _printFlag = false, _canFlag = true;
        private DateTime fromDate, toDate;

        private IniFile ini;
        private AmountInWords aw;

        private BackgroundWorker bwPrint = new BackgroundWorker();

        public PrintBillView()
        {
            InitializeComponent();

            this.bwPrint.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwPrint_DoWork);
            this.bwPrint.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwPrint_RunWorkerCompleted);
            this.bwPrint.WorkerSupportsCancellation = false;
            this.bwPrint.WorkerReportsProgress = false;
        }

        private void RadWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.cBoxCustomer.ItemsSource = DataServices.DOCustomer.GetAllCustomer();

            this.biMain.IsBusy = false;
            this._loaded = true;

            this.SetSelectionOnFocus(Telerik.Windows.Controls.SelectionOnFocus.SelectAll);

            ini = new IniFile();

            try
            {
                this.rdpFrom.SelectedValue = Convert.ToDateTime(ini.ReadIni()[6]);
                this.rdpTo.SelectedValue = Convert.ToDateTime(ini.ReadIni()[7]);
                this.rdpBillDate.SelectedValue = Convert.ToDateTime(ini.ReadIni()[8]);
            }
            catch
            { }

            _printFlag = true;
            _canFlag = true;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
            {
                this.cBoxCustomer.Focus();
            }));
        }

        private void SetSelectionOnFocus(Telerik.Windows.Controls.SelectionOnFocus selectionOnFocus)
        {
            if (!this._loaded)
            {
                return;
            }

            this.rdpFrom.SelectionOnFocus = selectionOnFocus;
            this.rdpTo.SelectionOnFocus = selectionOnFocus;
        }

        //Start --------------------------- Print Operation -----------------------------------

        public static RoutedCommand cmdPrint = new RoutedCommand();

        public void CanExecuteCmdPrint(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this._printFlag == true)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private DateTime billFrom, billTo, billDate;
        private string billNo;
        private long cid;

        public void ExecuteCmdPrint(object sender, ExecutedRoutedEventArgs e)
        {
            this.cBoxCustomer.GetBindingExpression(RadComboBox.SelectedIndexProperty).UpdateSource();
            this.rdpFrom.GetBindingExpression(RadDatePicker.SelectedValueProperty).UpdateSource();
            this.rdpTo.GetBindingExpression(RadDatePicker.SelectedValueProperty).UpdateSource();
            
            this.txtBillNo.GetBindingExpression(RadMaskedTextBox.ValueProperty).UpdateSource();
            this.rdpBillDate.GetBindingExpression(RadDatePicker.SelectedValueProperty).UpdateSource();

            if (!(Validation.GetHasError(this.rdpFrom) | Validation.GetHasError(this.rdpTo) | Validation.GetHasError(this.cBoxCustomer) | Validation.GetHasError(this.rdpBillDate) | Validation.GetHasError(this.txtBillNo)))
            {
                if (!this.bwPrint.IsBusy)
                {
                    this.biMain.IsBusy = true;
                    this._printFlag = false;
                    this._canFlag = false;

                    billFrom = Convert.ToDateTime(this.rdpFrom.SelectedValue);
                    billTo = Convert.ToDateTime(this.rdpTo.SelectedValue);
                    billDate = Convert.ToDateTime(this.rdpBillDate.SelectedValue);
                    billNo = this.txtBillNo.Value.ToString();

                    DataServices.Customer cust = this.cBoxCustomer.SelectedItem as DataServices.Customer;

                    this.bwPrint.RunWorkerAsync(cust);
                }
            }
        }

        private void bwPrint_DoWork(object sender, DoWorkEventArgs e)
        {
            DataServices.Customer cust = e.Argument as DataServices.Customer;

            if (cust.Name=="Local")
            {
            }
            else
            {
                Report.DataModel.MainBillHeader mbh = new Report.DataModel.MainBillHeader();

                easyBilling.Report.View.BillInvoice bi = new Report.View.BillInvoice();

                List<Report.DataModel.MainBillDetail> listMainBillDetail = DOPrintBill.GetMainBillDetail(this.cid, this.billFrom, this.billTo);

                mbh.GrandTotal = listMainBillDetail.Sum(b => b.NetAmt);
                mbh.GrandTotal = Math.Round(mbh.GrandTotal);

                AmountInWords aw = new Helper.AmountInWords();
                mbh.TotalInWords = aw.changeCurrencyToWords(Convert.ToDouble(mbh.GrandTotal));

                mbh.BillNo = this.billNo;
                mbh.BillDate = this.billDate;
                mbh.BillFrom = this.billFrom;
                mbh.BillTo = this.billTo;

                mbh.CustomerName = cust.Name;
                mbh.Address = cust.Address;
                mbh.ContactNo = cust.ContactNo;

                //bi.ods_BillHeader.DataSource = listBillView.First();
                //bi.ods_BillDetail.DataSource = listBillView;
            }
        }

        private void bwPrint_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //------------------- Write Ini Data --------------------------------
            string[] Value_s1 = new string[3];

            Value_s1[0] = Convert.ToDateTime(this.rdpFrom.SelectedValue).ToShortDateString();
            Value_s1[1] = Convert.ToDateTime(this.rdpTo.SelectedValue).ToShortDateString();
            Value_s1[2] = Convert.ToDateTime(this.rdpBillDate.SelectedValue).ToShortDateString();

            ini = new IniFile();
            ini.WriteIniOnlyBillPrint(Value_s1);
            //------------------- Write Ini Data --------------------------------
        }

        //End --------------------------- Print Operation -----------------------------------
                            
                              //----------------------------------------

        //Start ------------------------- Cancel Operation -----------------------------------

        public static RoutedCommand cmdCancel = new RoutedCommand();

        public void CanExecuteCmdCancel(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this._canFlag == true)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        public void ExecuteCmdCancel(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        //End ------------------------- Cancel Operation -----------------------------------
    }
}
