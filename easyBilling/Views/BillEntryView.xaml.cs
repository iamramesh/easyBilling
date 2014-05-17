using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using easyBilling.DataServices;
using Telerik.Windows.Controls;
using easyBilling.Helper;

namespace easyBilling.Views
{
    /// <summary>
    /// Interaction logic for BillEntryView.xaml
    /// </summary>
    public partial class BillEntryView : UserControl
    {
        private bool _newFlag = false, _saveFlag = false, _delFlag = false, _printFlag = false;

        private MainWindow mw;

        private BackgroundWorker bwNew = new BackgroundWorker();
        private BackgroundWorker bwSave = new BackgroundWorker();
        private BackgroundWorker bwUpdate = new BackgroundWorker();
        private BackgroundWorker bwSearch = new BackgroundWorker();
        private BackgroundWorker bwRetrieve = new BackgroundWorker();
        private BackgroundWorker bwDelete = new BackgroundWorker();
        private BackgroundWorker bwPrint = new BackgroundWorker();

        private DispatcherTimer tmMsg;
        private Helper.IniFile ini;

        private List<DataServices.Local> listLocal = new List<Local>();

        public BillEntryView(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;

            this.bwNew.DoWork += new DoWorkEventHandler(bwNew_DoWork);
            bwNew.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwNew_RunWorkerCompleted);
            this.bwNew.WorkerSupportsCancellation = false;
            this.bwNew.WorkerReportsProgress = false;

            this.bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);
            bwSave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);
            this.bwSave.WorkerSupportsCancellation = false;
            this.bwSave.WorkerReportsProgress = false;

            this.bwUpdate.DoWork += new DoWorkEventHandler(bwUpdate_DoWork);
            bwUpdate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwUpdate_RunWorkerCompleted);
            this.bwUpdate.WorkerSupportsCancellation = false;
            this.bwUpdate.WorkerReportsProgress = false;

            this.bwSearch.DoWork += new DoWorkEventHandler(bwSearch_DoWork);
            bwSearch.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSearch_RunWorkerCompleted);
            this.bwSearch.WorkerSupportsCancellation = false;
            this.bwSearch.WorkerReportsProgress = false;

            this.bwRetrieve.DoWork += new DoWorkEventHandler(bwRetrieve_DoWork);
            bwRetrieve.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwRetrieve_RunWorkerCompleted);
            this.bwRetrieve.WorkerSupportsCancellation = false;
            this.bwRetrieve.WorkerReportsProgress = false;

            this.bwDelete.DoWork += new DoWorkEventHandler(bwDelete_DoWork);
            bwDelete.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwDelete_RunWorkerCompleted);
            this.bwDelete.WorkerSupportsCancellation = false;
            this.bwDelete.WorkerReportsProgress = false;

            this.bwPrint.DoWork += new DoWorkEventHandler(bwPrint_DoWork);
            bwPrint.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwPrint_RunWorkerCompleted);
            this.bwPrint.WorkerSupportsCancellation = false;
            this.bwPrint.WorkerReportsProgress = false;

            tmMsg = new DispatcherTimer();
            tmMsg.Interval = TimeSpan.FromSeconds(5);
            tmMsg.Tick += new EventHandler(tmMsg_Tick);
        }

        void tmMsg_Tick(object sender, EventArgs e)
        {
            tmMsg.Stop();
            this.txtMsg.Text = "";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _newFlag = true;
            _saveFlag = false;
            _delFlag = false;
            _printFlag = false;

            this.stkMain.IsEnabled = false;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => 
            { 
                this.btnAdd.Focus(); 
            }));
        }

        //-----------------------Start - New Button Opearation ------------------------------------- 
        public static RoutedCommand cmdAdd = new RoutedCommand();

        public void CanExecuteCmdAdd(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this._newFlag == true)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        public void ExecuteCmdAdd(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.bwNew.IsBusy)
            {
                this.biMain.IsBusy = true;
                this.bwNew.RunWorkerAsync();
            }
        }

        void bwNew_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] rl = new object[4];

            rl[0] = DataOperations.GetAllStockItem();
            rl[1] = DataOperations.GetVoucherNo(1);
            rl[2] = DOCustomer.GetAllCustomer();
            rl[3] = DOCustomer.GetAllLocal();

            e.Result = rl;
        }
        
        void bwNew_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Error != null))
            {
                RadWindow.Alert("Error: " + e.Error.Message);

                _newFlag = false;
                _saveFlag = false;
                _delFlag = false;
                _printFlag = false;

                this.stkMain.IsEnabled = false;
                this.biMain.IsBusy = false;
            }
            else
            {
                object[] rl = e.Result as object[];

                this.listBillDetail.Clear();
                this.rgItem.ItemsSource = null;
                this.rgItem.IsReadOnly = false;

                this.cBoxItemName.ItemsSource = null;
                this.cBoxItemName.ItemsSource = rl[0] as IEnumerable<StockItem>;

                this.txtBillNo.Value = rl[1].ToString();
                this.txtBillDate.SelectedValue = System.DateTime.Now;

                this.cBoxCustName.ItemsSource = null;
                this.cBoxCustName.ItemsSource = rl[2] as IEnumerable<Customer>;

                this.cBoxContactNo.ItemsSource = null;
                this.cBoxContactNo.Text = null;
                this.txtAddress.Value = null;

                this.txtItemQty.Value = 0;
                this.txtRate.Value = 0;
                this.txtAmt.Value = 0;

                listLocal.Clear();
                listLocal = rl[3] as List<Local>;

                _newFlag = false;
                _saveFlag = true;
                _delFlag = false;
                _printFlag = false;

                this.stkMain.IsEnabled = true;
                this.biMain.IsBusy = false;

                try
                {
                    this.cBoxCustName.SelectedValue = DOCustomer.GetCustomerByName("Local").CustomerId;
                }
                catch
                {
                    RadWindow.Alert("Local Customer is not available.");

                    _newFlag = false;
                    _saveFlag = false;
                    _delFlag = false;
                    _printFlag = false;

                    this.stkMain.IsEnabled = false;
                    this.biMain.IsBusy = false;

                    return;
                }

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.cBoxContactNo.Focus();
                }));
            }
        }
        
        //-----------------------End - New Button Opearation ------------------------------------- 

                    //-------------------------------------------------//

        //-----------------------Start - Clear Button Opearation ------------------------------------- 

        public static RoutedCommand cmdClear = new RoutedCommand();

        public void CanExecuteCmdClear(object sender, CanExecuteRoutedEventArgs e)
        {    
            e.CanExecute = true;
        }

        public void ExecuteCmdClear(object sender, ExecutedRoutedEventArgs e)
        {
            this.txtBillNo.Value = null;
            this.txtBillDate.SelectedDate = null;

            this.cBoxCustName.ItemsSource = null;
            this.cBoxContactNo.ItemsSource = null;
            this.cBoxContactNo.Text = null;
            this.txtAddress.Value = null;

            this.txtItemQty.Value = 0;
            this.txtRate.Value = 0;
            this.txtAmt.Value = 0;
            this.cBoxItemName.ItemsSource = null;
            this.rgItem.ItemsSource = null;

            _newFlag = true;
            _saveFlag = false;
            _delFlag = false;
            _printFlag = false;

            this.stkMain.IsEnabled = false;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
            {
                this.btnAdd.Focus();
            }));
        }

        //-----------------------End - Clear Button Opearation ------------------------------------- 

                                //-------------------------------------------------//

        //-----------------------Start - Save Button Opearation ------------------------------------- 

        public static RoutedCommand cmdSave = new RoutedCommand();

        public void CanExecuteCmdSave(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this._saveFlag == true)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private Local newLocal;

        public void ExecuteCmdSave(object sender, ExecutedRoutedEventArgs e)
        {
            this.txtBillNo.GetBindingExpression(RadMaskedTextBox.ValueProperty).UpdateSource();
            this.txtBillDate.GetBindingExpression(RadDatePicker.SelectedValueProperty).UpdateSource();

            try
            {
                this.cBoxContactNo.GetBindingExpression(RadComboBox.TextProperty).UpdateSource();
            }
            catch 
            {
                try
                {
                    if (!String.IsNullOrEmpty(this.cBoxContactNo.Text))
                    {
                        try
                        {
                            long cn = Convert.ToInt64(this.cBoxContactNo.Text);
                        }
                        catch
                        {
                            RadWindow.Alert("Please Check Contact Number.");
                            return;
                        }
                    }
                }
                catch
                { }
            }

            if (!(Validation.GetHasError(this.txtBillNo) | Validation.GetHasError(this.txtBillDate) | Validation.GetHasError(this.cBoxContactNo)))
            {
                if (this.rgItem.Items.Count > 0)
                {
                    if (_newFlag == false)
                    {
                        BillHeader bh = new BillHeader();
                        bh.VoucherNo = this.txtBillNo.Value.ToString();
                        bh.VoucherDate = Convert.ToDateTime(this.txtBillDate.SelectedValue);
                        bh.NetQty = Convert.ToDecimal(this.txtNetQty.Text);
                        bh.NetRate = Convert.ToDecimal(this.txtNetRate.Text);
                        bh.NetAmt = Convert.ToDecimal(this.txtNetAmt.Text);

                        try
                        {
                            Customer cust = this.cBoxCustName.SelectedItem as Customer;

                            if (cust.Name == "Local")
                            {
                                bh.IsLocal = true;
                            }
                            else
                            {
                                bh.IsLocal = false;
                                bh.CustomerId = cust.CustomerId;
                            }
                        }
                        catch 
                        {
                            bh.CustomerId = 0;
                        }

                        newLocal = new Local();

                        if (bh.IsLocal == true)
                        {
                            try
                            {
                                newLocal.Name = this.txtLocalName.Value.ToString();
                            }
                            catch
                            {
                                newLocal.Name = null;
                            }

                            try
                            {
                                newLocal.ContactNo = this.cBoxContactNo.Text.ToString();
                            }
                            catch
                            {
                                newLocal.ContactNo = null;
                            }

                            try
                            {
                                newLocal.Address = this.txtAddress.Value.ToString();
                            }
                            catch
                            {
                                newLocal.Address = null;
                            }
                        }
                        else
                        {
                            newLocal.ContactNo = null;
                        }

                        if (!this.bwSave.IsBusy)
                        {
                            this.biMain.IsBusy = true;

                            _newFlag = false;
                            _saveFlag = false;
                            _delFlag = false;
                            _printFlag = false;

                            this.bwSave.RunWorkerAsync(bh);
                        }
                    }
                    else if (_newFlag == true)
                    {
                        BillHeader bh = new BillHeader();
                        bh.BillHeaderId = BHid;
                        bh.VoucherNo = this.txtBillNo.Value.ToString();
                        bh.VoucherDate = Convert.ToDateTime(this.txtBillDate.SelectedValue);
                        bh.NetQty = Convert.ToDecimal(this.txtNetQty.Text);
                        bh.NetRate = Convert.ToDecimal(this.txtNetRate.Text);
                        bh.NetAmt = Convert.ToDecimal(this.txtNetAmt.Text);

                        try
                        {
                            Customer cust = this.cBoxCustName.SelectedItem as Customer;

                            if (cust.Name == "Local")
                            {
                                bh.IsLocal = true;
                            }
                            else
                            {
                                bh.IsLocal = false;
                                bh.CustomerId = cust.CustomerId;
                            }
                        }
                        catch
                        {
                            bh.CustomerId = 0;
                        }

                        newLocal = new Local();

                        if (bh.IsLocal == true)
                        {
                            try
                            {
                                newLocal.Name = this.txtLocalName.Value.ToString();
                            }
                            catch
                            {
                                newLocal.Name = null;
                            }

                            try
                            {
                                newLocal.ContactNo = this.cBoxContactNo.Text.ToString();
                            }
                            catch
                            {
                                newLocal.ContactNo = null;
                            }

                            try
                            {
                                newLocal.Address = this.txtAddress.Value.ToString();
                            }
                            catch
                            {
                                newLocal.Address = null;
                            }
                        }
                        else
                        {
                            newLocal.ContactNo = null;
                        }

                        if (!this.bwUpdate.IsBusy)
                        {
                            this.biMain.IsBusy = true;

                            _newFlag = false;
                            _saveFlag = false;
                            _delFlag = false;
                            _printFlag = false;

                            this.bwUpdate.RunWorkerAsync(bh);
                        }
                    }
                }
                else
                {
                    RadWindow.Alert("Please Check Item Details.");
                }
            }
        }

        void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            List<BillDetail> listBD = new List<BillDetail>();

            foreach (var lbd in listBillDetail)
            {
                BillDetail bd = new BillDetail();
                bd.StockItemId = lbd.StockItemId;
                bd.Qty = lbd.Qty;
                bd.Rate = lbd.Rate;
                bd.Amount = lbd.Amount;
                bd.Deleted = lbd.Deleted;

                listBD.Add(bd);
            }

            BillHeader bh = e.Argument as BillHeader;

            if (bh.IsLocal == true)
            {
                if (newLocal.ContactNo != null)
                {
                    e.Result = DataOperations.AddBillHeader(bh, listBD, newLocal);
                }
                else
                {
                    e.Result = DataOperations.AddBillHeader(bh, listBD);
                }
            }
            else
            {
                e.Result = DataOperations.AddBillHeader(bh, listBD);
            }
        }

        void bwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Convert.ToInt64(e.Result) != 0)
            {
                this.txtMsg.Text = "New record is created successfully.";
                this.tmMsg.Start();

                _newFlag = true;
                _saveFlag = false;
                _delFlag = false;
                _printFlag = false;

                this.stkMain.IsEnabled = false;
                this.biMain.IsBusy = false;

                BHid = Convert.ToInt64(e.Result);

                RadWindow.Confirm("Do you want to Print Bill?", this.OnPrintClosed);
            }
            else
            {
                RadWindow.Alert("System problem in creating this new record.");

                _newFlag = false;
                _saveFlag = true;
                _delFlag = false;
                _printFlag = false;

                this.biMain.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.btnSave.Focus();
                }));
            }
        }

        private void OnPrintClosed(object sender, WindowClosedEventArgs e)
        {
            bool shouldPrint = e.DialogResult.HasValue ? e.DialogResult.Value : false;

            if (shouldPrint)
            {
                Print(BHid);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.btnAdd.Focus();
                }));
            }
        }

        void bwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            List<BillDetail> listBD = new List<BillDetail>();

            foreach (var lbd in listBillDetail)
            {
                BillDetail bd = new BillDetail();
                bd.BillDetailId = lbd.BillDetailId;
                bd.StockItemId = lbd.StockItemId;
                bd.Qty = lbd.Qty;
                bd.Rate = lbd.Rate;
                bd.Amount = lbd.Amount;
                bd.Deleted = lbd.Deleted;

                listBD.Add(bd);
            }

            BillHeader bh = e.Argument as BillHeader;

            if (bh.IsLocal == true)
            {
                if (newLocal.ContactNo != null)
                {
                    e.Result = DataOperations.UpdateBillHeader(bh, listBD, newLocal);
                }
                else
                {
                    e.Result = DataOperations.UpdateBillHeader(bh, listBD);
                }
            }
            else
            {
                e.Result = DataOperations.UpdateBillHeader(bh, listBD);
            }
        }

        void bwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Convert.ToInt64(e.Result) != 0)
            {
                this.txtMsg.Text = "Selected record is updated successfully";
                this.tmMsg.Start();

                _newFlag = true;
                _saveFlag = false;
                _delFlag = false;
                _printFlag = false;

                this.stkMain.IsEnabled = false;
                this.biMain.IsBusy = false;

                BHid = Convert.ToInt64(e.Result);
                RadWindow.Confirm("Do you want to Print Bill?", this.OnPrintClosed);
            }
            else
            {
                RadWindow.Alert("System problem in updating the Selected record.");

                _newFlag = true;
                _saveFlag = true;
                _delFlag = true;
                _printFlag = true;

                this.biMain.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.btnSave.Focus();
                }));
            }
        }

        //-----------------------End - Save Button Opearation ------------------------------------- 

                                //-------------------------------------------------//

        //-----------------------Start - Delete Button Opearation ------------------------------------- 

        public static RoutedCommand cmdDelete = new RoutedCommand();

        public void CanExecuteCmdDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this._delFlag == true)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        public void ExecuteCmdDelete(object sender, ExecutedRoutedEventArgs e)
        {
            RadWindow.Confirm("Are you Sure you want to Delete?", this.OnDeleteClosed);
        }

        private void OnDeleteClosed(object sender, WindowClosedEventArgs e)
        {
            bool shouldDelete = e.DialogResult.HasValue ? e.DialogResult.Value : false;

            if (shouldDelete)
            {
                if (!this.bwDelete.IsBusy)
                {
                    this.biMain.IsBusy = true;

                    _newFlag = false;
                    _saveFlag = false;
                    _delFlag = false;
                    _printFlag = false;

                    this.bwDelete.RunWorkerAsync();
                }
            }
        }

        void bwDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = DataOperations.DeleteBillHeader(BHid);
        }

        void bwDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((Boolean)e.Result)
            {
                this.txtMsg.Text = "Selected record is deleted successfully.";
                this.tmMsg.Start();

                this.txtBillNo.Value = null;
                this.txtBillDate.SelectedDate = null;

                this.cBoxCustName.ItemsSource = null;
                this.cBoxContactNo.ItemsSource = null;
                this.cBoxContactNo.Text = null;
                this.txtAddress.Value = null;

                this.txtItemQty.Value = 0;
                this.txtRate.Value = 0;
                this.txtAmt.Value = null;
                this.cBoxItemName.ItemsSource = null;
                this.rgItem.ItemsSource = null;

                _newFlag = true;
                _saveFlag = false;
                _delFlag = false;
                _printFlag = false;

                this.txtSearch.IsEnabled = false;
                this.rgSearch.IsEnabled = false;
                this.stkMain.IsEnabled = false;
                this.biMain.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)(() =>
                {
                    this.btnSearch.Focus();
                }));
            }
            else
            {
                this.txtMsg.Text = "System problem in deleting the selected record.";
                this.tmMsg.Start();

                _newFlag = true;
                _saveFlag = true;
                _delFlag = true;
                _printFlag = true;

                this.stkMain.IsEnabled = true;
                this.biMain.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)(() =>
                {
                    this.btnDelete.Focus();
                }));
            }
        }

        //-----------------------End - Delete Button Opearation ------------------------------------- 

                        //-------------------------------------------------//

        //-----------------------Start - Print Button Opearation ------------------------------------- 

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

        public void ExecuteCmdPrint(object sender, ExecutedRoutedEventArgs e)
        {
            Print(BHid);
        }

        private void Print(long BHid)
        {
            if (BHid != 0 && BHid != null)
            {
                if (!this.bwPrint.IsBusy)
                {
                    this.biMain.IsBusy = true;
                    this.bwPrint.RunWorkerAsync(BHid);
                }   
            }
        }

        void bwPrint_DoWork(object sender, DoWorkEventArgs e)
        {
            easyBilling.Report.View.BillInvoice bi = new Report.View.BillInvoice();

            List<easyBilling.Report.DataModel.BillView> listBillView = DataOperations.GetBillView(Convert.ToInt64(e.Argument));

            bi.ods_BillHeader.DataSource = listBillView.First();
            bi.ods_BillDetail.DataSource = listBillView;

            ini = new IniFile();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)(() =>    
            {
                if (Convert.ToInt32(ini.ReadIni()[3].ToString())==1)
                {
                    ReportView.ReportView rv = new ReportView.ReportView();
                    bi.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;

                    double hgt = Convert.ToDouble(ini.ReadIni()[1]) + (Convert.ToDouble(ini.ReadIni()[2]) * Convert.ToDouble(listBillView.Count));
                    //double hgt = 4.5 + (0.25 * Convert.ToDouble(listBillView.Count));
                    bi.PageSettings.PaperSize = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3), Telerik.Reporting.Drawing.Unit.Inch(hgt));

                    Telerik.Reporting.IReportDocument myReport = bi;
                    // Obtain the settings of the default printer
                    System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
                    // The standard print controller comes with no UI
                    System.Drawing.Printing.PrintController standardPrintController = new System.Drawing.Printing.StandardPrintController();
                    // Print the report using the custom print controller
                    Telerik.Reporting.Processing.ReportProcessor reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
                    reportProcessor.PrintController = standardPrintController;

                    Telerik.Reporting.InstanceReportSource instanceReportSource = new Telerik.Reporting.InstanceReportSource();
                    instanceReportSource.ReportDocument = myReport;

                    reportProcessor.PrintReport(instanceReportSource, printerSettings);
                }
                else
                {
                    ReportView.ReportView rv = new ReportView.ReportView();
                    bi.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;

                    double hgt = Convert.ToDouble(ini.ReadIni()[1]) + (Convert.ToDouble(ini.ReadIni()[2]) * Convert.ToDouble(listBillView.Count));
                    //double hgt = 4.5 + (0.25 * Convert.ToDouble(listBillView.Count));
                    bi.PageSettings.PaperSize = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(63.34), Telerik.Reporting.Drawing.Unit.Inch(hgt));

                    rv.rv1.Report = bi;
                    rv.Show();
                }    
            }));

            //Telerik.Reporting.IReportDocument myReport = bi;
            //// Obtain the settings of the default printer
            //System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
            //// The standard print controller comes with no UI
            //System.Drawing.Printing.PrintController standardPrintController = new System.Drawing.Printing.StandardPrintController();
            //// Print the report using the custom print controller
            //Telerik.Reporting.Processing.ReportProcessor reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
            //reportProcessor.PrintController = standardPrintController;

            //Telerik.Reporting.InstanceReportSource instanceReportSource = new Telerik.Reporting.InstanceReportSource();
            //instanceReportSource.ReportDocument = myReport;

            //reportProcessor.PrintReport(instanceReportSource, printerSettings);
        }
 
        void bwPrint_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                RadWindow.Alert("Error: " + e.Error.Message);
                this.biMain.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.btnPrint.Focus();
                }));
            }
            else
            {
                this.biMain.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.btnPrint.Focus();
                }));
            }
        }

        //-----------------------End - Print Button Opearation -------------------------------------

                      //-------------------------------------------------//

        //-----------------------Start - Search Opearation ------------------------------------- 

        public static RoutedCommand cmdRefresh = new RoutedCommand();

        public void CanExecuteCmdRefresh(object sender, CanExecuteRoutedEventArgs e)
        {    
            e.CanExecute = true;
        }

        public void ExecuteCmdRefresh(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.bwSearch.IsBusy)
            {
                this.rgSearch.IsBusy = true;
                this.bwSearch.RunWorkerAsync();
            }
        }

        void bwSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = DataOperations.GetAllBillHeader();
        }

        void bwSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                RadWindow.Alert("Error: " + e.Error.Message);
                this.biMain.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.btnSearch.Focus();
                }));
            }
            else
            {
                this.txtSearch.IsEnabled = true;
                this.rgSearch.IsEnabled = true;
                this.rgSearch.ItemsSource = e.Result;
                this.rgSearch.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.txtSearch.Focus();
                }));
            }
        }

        private CustomFilterDescriptor customFilterDescriptor;
        public CustomFilterDescriptor CustomFilterDescriptor
        {
            get
            {
                if (this.customFilterDescriptor == null)
                {
                    this.customFilterDescriptor = new CustomFilterDescriptor(this.rgSearch.Columns.OfType<Telerik.Windows.Controls.GridViewDataColumn>());
                    this.rgSearch.FilterDescriptors.Add(this.customFilterDescriptor);
                }
                return this.customFilterDescriptor;
            }
        }

        private void txtSearch_ValueChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                string sText = (sender as Telerik.Windows.Controls.RadMaskedTextBox).Value.ToString();
                this.CustomFilterDescriptor.FilterValue = sText;
            }
            catch
            {}
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Tab)
            //{
            //    if (this.rgSearch.Items.Count == 1)
            //    {
            //        e.Handled = true;

            //        this.txtBillDate.Focus();
            //    }
            //    else if (this.rgSearch.Items.Count > 1)
            //    {
            //        e.Handled = false;
            //        return;
            //    }
            //    else
            //    {
            //        e.Handled = true;
            //        return;
            //    }
            //}
        }

        private void rgSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!this.bwRetrieve.IsBusy)
            {
                this.biMain.IsBusy = true;

                BillHeader bh = this.rgSearch.SelectedItem as BillHeader;

                this.bwRetrieve.RunWorkerAsync(bh);
            }
        }

        private void rgSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                e.Handled = true;
                this.txtSearch.Focus();

                return;
            }
            else if (e.Key == Key.Tab)
            {
                if (this.rgSearch.Items.Count >= 1)
                {
                    e.Handled = true;

                    this.txtBillDate.Focus();

                    return;
                }

                e.Handled = false;
                return;
            }
            else if (e.Key == Key.Space)
            {
                e.Handled = true;

                if (!this.bwRetrieve.IsBusy)
                {
                    this.biMain.IsBusy = true;

                    BillHeader bh = this.rgSearch.SelectedItem as BillHeader;

                    this.bwRetrieve.RunWorkerAsync(bh);
                }
            }
        }

        void bwRetrieve_DoWork(object sender, DoWorkEventArgs e)
        {
            BillHeader bh = e.Argument as BillHeader;
            object[] obj = new object[5];

            obj[0] = DataOperations.GetBillHeaderByBHId(bh.BillHeaderId);
            obj[1] = DataOperations.GetVW_BillDetail_UpdateByBHid(bh.BillHeaderId);
            obj[2] = DataOperations.GetAllStockItem();

            obj[3] = DOCustomer.GetAllCustomer();
            obj[4] = DOCustomer.GetAllLocal();

            e.Result = obj;
        }

        long BHid = 0;

        void bwRetrieve_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error!= null)
            {
                RadWindow.Alert("Error: " + e.Error.Message);

                this.biMain.IsBusy = false;
            }
            else
            {
                object[] rl = e.Result as object[];

                this.listBillDetail.Clear();

                foreach (var bd in rl[1] as List<VW_BillDetail_Update>)
                {
                    VW_BillDetail_New bdn = new VW_BillDetail_New();
                    bdn.BillDetailId = bd.BillDetailId;
                    bdn.StockItemId = Convert.ToInt64(bd.StockItemId);
                    bdn.ItemName = bd.ItemName;
                    bdn.Qty = Convert.ToDecimal(bd.Qty);
                    bdn.Rate = Convert.ToDecimal(bd.Rate);
                    bdn.Amount = Convert.ToDecimal(bd.Amount);
                    bdn.Deleted = Convert.ToBoolean(bd.Deleted);

                    listBillDetail.Add(bdn);
                }

                this.rgItem.ItemsSource = null;
                this.rgItem.ItemsSource = listBillDetail;

                this.cBoxItemName.ItemsSource = null;
                this.cBoxItemName.ItemsSource = rl[2] as IEnumerable<StockItem>;

                this.cBoxCustName.ItemsSource = null;
                this.cBoxCustName.ItemsSource = rl[3] as IEnumerable<Customer>;

                listLocal.Clear();
                listLocal = rl[4] as List<Local>;

                this.cBoxContactNo.ItemsSource = null;
                this.cBoxContactNo.ItemsSource = listLocal;

                this.txtLocalName.Value = null;
                this.txtAddress.Value = null;

                this.gridDC.DataContext = null;
                this.gridDC.DataContext = rl[0];

                BillHeader bh = rl[0] as BillHeader;

                this.BHid = bh.BillHeaderId;
                this.txtBillNo.Value = bh.VoucherNo;
                this.txtBillDate.SelectedValue = bh.VoucherDate;

                this.txtItemQty.Value = 0;
                this.txtRate.Value = 0;
                this.txtAmt.Value = 0;

                if (bh.IsLocal == true)
                {
                    try
                    {
                        this.cBoxCustName.SelectedValue = DOCustomer.GetCustomerByName("Local").CustomerId;
                    }
                    catch
                    {
                        RadWindow.Alert("Local Customer is not available.");

                        this.biMain.IsBusy = false;

                        _newFlag = true;
                        _saveFlag = false;
                        _delFlag = false;
                        _printFlag = false;

                        return;
                    }

                    this.cBoxContactNo.SelectedValue = bh.CustomerId;
                }
                else
                {
                    this.cBoxCustName.SelectedValue = bh.CustomerId;
                }

                _newFlag = true;
                _saveFlag = true;
                _delFlag = true;
                _printFlag = true;

                this.stkMain.IsEnabled = true;
                this.biMain.IsBusy = false;

                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                //{
                //    this.txtBillDate.Focus();
                //}));
            }
        }

        //-----------------------End - Search Opearation ------------------------------------- 

                        //-------------------------------------------------//

        //-----------------------Start - Customer Details Module ------------------------------------- 

        private void cBoxCustName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Customer cust = this.cBoxCustName.SelectedItem as Customer;

                if (cust.Name == "Local")
                {
                    this.stkLocal.Visibility = Visibility.Visible;

                    this.cBoxContactNo.ItemsSource = listLocal;

                    this.txtLocalName.Value = null;
                    this.cBoxContactNo.Text = null;
                    this.txtAddress.Value = null;
                }
                else
                {
                    this.stkLocal.Visibility = Visibility.Collapsed;

                    this.cBoxContactNo.Text = cust.ContactNo;
                    this.txtAddress.Value = cust.Address;
                }
            }
            catch
            { }
        }

        private void txtContactNo_ValueChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //this.txtLocalName.Value = this.txtLocalName.Value.ToString() + this.txtContactNo.Value.ToString();
            RadMaskedTextBox mtb = sender as RadMaskedTextBox;
            try
            {
                //mtb.Value
                DataServices.Local local = listLocal.Where(l => l.ContactNo == mtb.Value.ToString()).First();

                this.txtLocalName.Value = local.Name;
                this.txtAddress.Value = local.Address;
            }
            catch
            { }
        }

        string cBoxText;

        private void cBoxContactNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.LeftShift | e.Key != Key.RightShift | e.Key != Key.Tab)
            {
                this.cBoxContactNo.IsDropDownOpen = true;
            }

            cBoxText = this.cBoxContactNo.Text;
        }

        private void cBoxContactNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Local local = this.cBoxContactNo.SelectedItem as Local;

                //DataServices.Local local = listLocal.Where(l => l.ContactNo == mtb.Value.ToString()).First();

                this.txtLocalName.Value = local.Name;
                this.txtAddress.Value = local.Address;
            }
            catch
            { }
        }

        private void cBoxContactNo_LostFocus(object sender, RoutedEventArgs e)
        {
            this.cBoxContactNo.Text = cBoxText;
            cBoxText = null;

            try
            {
                Local local = this.cBoxContactNo.SelectedItem as Local;

                this.txtLocalName.Value = local.Name;
                this.txtAddress.Value = local.Address;
            }
            catch
            { }
        }

        //-----------------------End - Customer Details Module ------------------------------------- 

                        //-------------------------------------------------//

        //-----------------------Start - Item Details Module ------------------------------------- 

        List<VW_BillDetail_New> listBillDetail = new List<VW_BillDetail_New>();

        private void txtRate_ValueChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                this.txtAmt.Value = Convert.ToDecimal(this.txtItemQty.Value) * Convert.ToDecimal(this.txtRate.Value);
            }
            catch
            { }
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            this.cBoxItemName.GetBindingExpression(RadComboBox.SelectedIndexProperty).UpdateSource();
            this.txtItemQty.GetBindingExpression(RadNumericUpDown.ValueProperty).UpdateSource();
            this.txtRate.GetBindingExpression(RadMaskedTextBox.ValueProperty).UpdateSource();
            this.txtAmt.GetBindingExpression(RadMaskedTextBox.ValueProperty).UpdateSource();

            if (!(Validation.GetHasError(this.cBoxItemName) | Validation.GetHasError(this.txtItemQty) | Validation.GetHasError(this.txtRate) | Validation.GetHasError(this.txtAmt)))
            {
                StockItem sic = this.cBoxItemName.SelectedItem as StockItem;

                VW_BillDetail_New bdn = new VW_BillDetail_New();
                bdn.StockItemId = sic.StockItemId;
                bdn.ItemName = sic.ItemName;
                bdn.Qty = Convert.ToDecimal(this.txtItemQty.Value);
                bdn.Rate = Convert.ToDecimal(this.txtRate.Value);
                bdn.Amount = Convert.ToDecimal(this.txtAmt.Value);
                bdn.Deleted = false;

                if (listBillDetail.Where(a => a.Deleted == false && a.ItemName == bdn.ItemName).Count() > 0)
                {
                    e.Handled = true;
                    RadWindow.Alert("Item Already Exist.");
                    this.cBoxItemName.Focus();

                    return;
                }

                listBillDetail.Add(bdn);

                this.rgItem.ItemsSource = listBillDetail.Where(a => a.Deleted == false).ToList();

                this.cBoxItemName.SelectedIndex = -1;
                this.txtItemQty.Value = 0;
                this.txtRate.Value = 0;

                this.cBoxItemName.Focus();
            }
        }

        protected IEnumerable<Object> itemsToBeDeleted;

        private void rgItem_Deleting(object sender, Telerik.Windows.Controls.GridViewDeletingEventArgs e)
        {
            itemsToBeDeleted = e.Items;
            e.Cancel = true;
            RadWindow.Confirm("Are you Sure you want to Delete?", this.OnRadWindowClosed);
        }

        private void OnRadWindowClosed(object sender, WindowClosedEventArgs e)
        {
            bool shouldDelete = e.DialogResult.HasValue ? e.DialogResult.Value : false;

            if (shouldDelete)
            {
                foreach (var itbd in itemsToBeDeleted)
                {
                    VW_BillDetail_New bdn = itbd as VW_BillDetail_New;
                    listBillDetail.Remove(bdn);
                    
                    bdn.Deleted = true;
                    listBillDetail.Add(bdn);
                }

                this.rgItem.ItemsSource = listBillDetail.Where(a => a.Deleted == false).ToList();
            }
        }

        //-----------------------End - Item Details Module ------------------------------------- 

    }
}
