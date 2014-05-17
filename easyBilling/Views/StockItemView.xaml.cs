using System;
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
    /// Interaction logic for StockItemView.xaml
    /// </summary>
    public partial class StockItemView : UserControl
    {
        private bool _newFlag = false, _saveFlag = false, _delFlag = false;

        private MainWindow mw;

        private BackgroundWorker bwNew = new BackgroundWorker();
        private BackgroundWorker bwSave = new BackgroundWorker();
        private BackgroundWorker bwUpdate = new BackgroundWorker();
        private BackgroundWorker bwSearch = new BackgroundWorker();
        private BackgroundWorker bwRetrieve = new BackgroundWorker();
        private BackgroundWorker bwDelete = new BackgroundWorker();

        private DispatcherTimer tmMsg;

        public StockItemView(MainWindow mw)
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
            e.Result  = DataOperations.GetVoucherNo(3);
        }

        void bwNew_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Error != null))
            {
                RadWindow.Alert("Error: " + e.Error.Message);

                _newFlag = false;
                _saveFlag = false;
                _delFlag = false;

                this.stkMain.IsEnabled = false;
                this.biMain.IsBusy = false;
            }
            else
            {
                this.txtItemNo.Value = e.Result.ToString();

                this.txtItemName.Value = null;
                this.txtRemarks.Value = null;                

                _newFlag = false;
                _saveFlag = true;
                _delFlag = false;

                this.stkMain.IsEnabled = true;
                this.biMain.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.txtItemName.Focus();
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
            this.txtItemNo.Value = null;

            this.txtItemName.Value = null;
            this.txtRemarks.Value = null;

            _newFlag = true;
            _saveFlag = false;
            _delFlag = false;

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

        public void ExecuteCmdSave(object sender, ExecutedRoutedEventArgs e)
        {
            this.txtItemNo.GetBindingExpression(RadMaskedTextBox.ValueProperty).UpdateSource();
            this.txtItemName.GetBindingExpression(RadMaskedTextBox.ValueProperty).UpdateSource();

            if (!(Validation.GetHasError(this.txtItemNo) | Validation.GetHasError(this.txtItemName)))
            {
                if (_newFlag == false)
                {
                    StockItem si = new StockItem();
                    si.VoucherNo = this.txtItemNo.Value.ToString();
                    si.ItemName = this.txtItemName.Value.ToString();

                    try
                    {
                        si.Remarks = this.txtRemarks.Value.ToString();
                    }
                    catch
                    {
                        si.Remarks = null;
                    }

                    if (!this.bwSave.IsBusy)
                    {
                        this.biMain.IsBusy = true;

                        _newFlag = false;
                        _saveFlag = false;
                        _delFlag = false;

                        this.bwSave.RunWorkerAsync(si);
                    }
                }
                else if (_newFlag == true)
                {
                    StockItem si = new StockItem();
                    si.StockItemId = SIid;
                    si.VoucherNo = this.txtItemNo.Value.ToString();
                    si.ItemName = this.txtItemName.Value.ToString();

                    try
                    {
                        si.Remarks = this.txtRemarks.Value.ToString();
                    }
                    catch
                    {
                        si.Remarks = null;
                    }

                    if (!this.bwUpdate.IsBusy)
                    {
                        this.biMain.IsBusy = true;

                        _newFlag = false;
                        _saveFlag = false;
                        _delFlag = false;

                        this.bwUpdate.RunWorkerAsync(si);
                    }
                }
            }
        }

        void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = DataOperations.AddStockItem(e.Argument as StockItem);
        }

        void bwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Convert.ToBoolean(e.Result) == true)
            {
                this.txtMsg.Text = "New record is created successfully.";
                this.tmMsg.Start();

                _newFlag = true;
                _saveFlag = false;
                _delFlag = false;

                this.stkMain.IsEnabled = false;
                this.biMain.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.btnAdd.Focus();
                }));
            }
            else
            {
                RadWindow.Alert("System problem in creating this new record.");

                _newFlag = false;
                _saveFlag = true;
                _delFlag = false;

                this.biMain.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.btnSave.Focus();
                }));
            }
        }

        void bwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = DataOperations.UpdateStockItem(e.Argument as StockItem);
        }

        void bwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Convert.ToBoolean(e.Result) == true)
            {
                this.txtMsg.Text = "Selected record is updated successfully";
                this.tmMsg.Start();

                _newFlag = true;
                _saveFlag = false;
                _delFlag = false;

                this.stkMain.IsEnabled = false;
                this.biMain.IsBusy = false;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    this.btnAdd.Focus();
                }));
            }
            else
            {
                RadWindow.Alert("System problem in updating the Selected record.");

                _newFlag = true;
                _saveFlag = true;
                _delFlag = true;

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

                    this.bwDelete.RunWorkerAsync();
                }
            }
        }

        void bwDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = DataOperations.DeleteStockItem(SIid);
        }

        void bwDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((Boolean)e.Result)
            {
                this.txtMsg.Text = "Selected record is deleted successfully.";
                this.tmMsg.Start();

                this.txtItemNo.Value = null;

                this.txtItemName.Value = null;
                this.txtRemarks.Value = null;

                _newFlag = true;
                _saveFlag = false;
                _delFlag = false;

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
            e.Result = DataOperations.GetAllStockItem();
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
            { }
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

                StockItem si = this.rgSearch.SelectedItem as StockItem;

                this.bwRetrieve.RunWorkerAsync(si);
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

                    this.txtItemName.Focus();

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

                    StockItem si = this.rgSearch.SelectedItem as StockItem;

                    this.bwRetrieve.RunWorkerAsync(si);
                }
            }
        }

        void bwRetrieve_DoWork(object sender, DoWorkEventArgs e)
        {
            StockItem si = e.Argument as StockItem;

            e.Result = DataOperations.GetStockItemBySid(si.StockItemId);
        }

        long SIid = 0;

        void bwRetrieve_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                RadWindow.Alert("Error: " + e.Error.Message);

                this.biMain.IsBusy = false;
            }
            else
            {
                StockItem si = e.Result as StockItem;

                this.SIid = si.StockItemId;
                this.txtItemNo.Value = si.VoucherNo;

                this.txtItemName.Value = si.ItemName;
                this.txtRemarks.Value = si.Remarks;

                _newFlag = true;
                _saveFlag = true;
                _delFlag = true;

                this.stkMain.IsEnabled = true;
                this.biMain.IsBusy = false;

                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                //{
                //    this.txtBillDate.Focus();
                //}));
            }
        }

        //-----------------------End - Search Opearation ------------------------------------- 
    }
}
