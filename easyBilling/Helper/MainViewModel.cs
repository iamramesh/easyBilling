using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace easyBilling.Helper
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            this.Tabs = new ObservableCollection<TabViewModel>();
        }

        /// <summary>
        /// Gets the collection of tabs.
        /// </summary>
        public ObservableCollection<TabViewModel> Tabs
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds new tab item to the Tabs collection.
        /// </summary>
        public void AddItem(string h, UserControl ui, string imgu)
        {
            TabViewModel newTabItem = new TabViewModel(this);
            newTabItem.Header = h;
            newTabItem.UIControl = ui;
            newTabItem.ImgUrl = imgu;
            newTabItem.IsSelected = true;

            //if (sender != null)
            //{
            //    int insertIndex = this.Tabs.IndexOf(sender) + 1;
            //    this.Tabs.Insert(insertIndex, newTabItem);
            //}
            //else
            //{
            this.Tabs.Add(newTabItem);
            //}
        }

        public void AddItem(TabViewModel v)
        {
            this.Tabs.Add(v);
        }

        /// <summary>
        /// Removes an item from the Tabs collection.
        /// </summary>
        /// <param name="tabItem">The tab item.</param>
        public void RemoveItem(TabViewModel tabItem)
        {
            this.Tabs.Remove(tabItem);
        }
    }
}
