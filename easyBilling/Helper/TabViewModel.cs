using System;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using System.ComponentModel;

namespace easyBilling.Helper
{
    public class TabViewModel : INotifyPropertyChanged
    {
        private bool isSelected;
        private MainViewModel mainViewModel;

        public TabViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.mainViewModel.Tabs.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Tabs_CollectionChanged);

            this.RemoveItemCommand = new DelegateCommand(
                delegate
                {
                    this.mainViewModel.RemoveItem(this);
                },
                delegate
                {
                    return this.mainViewModel.Tabs.Count > 1;
                });
        }

        void Tabs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.RemoveItemCommand.InvalidateCanExecute();
        }

        public string Header
        {
            get;
            set;
        }

        public UserControl UIControl
        {
            get;
            set;
        }

        public string ImgUrl
        {
            get;
            set;
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        public DelegateCommand RemoveItemCommand { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
