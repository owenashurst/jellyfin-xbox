using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Jellyfin.Core.Models;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.ViewModels;

namespace Jellyfin.UserControls
{
    public sealed partial class MediaElementListUserControl
    {
        #region Properties

        public ListViewItem FirstElement
        {
            get => (ListViewItem)itemsContainer.ContainerFromIndex(0);
        }

        public ListViewItem SelectedElement
        {
            get
            {
                if (itemsContainer.SelectedIndex == -1)
                {
                    return null;
                }

                return (ListViewItem)itemsContainer.ContainerFromIndex(itemsContainer.SelectedIndex);
            } 
        }

        #endregion

        #region MediaSource Dependency Property

        public static readonly DependencyProperty MediaSourceDependency = DependencyProperty.Register("MediaSource", typeof(ObservableCollection<ModelBase>), typeof(MediaElementListUserControl), new PropertyMetadata(null));

        public ObservableCollection<ModelBase> MediaSource
        {
            get => (ObservableCollection<ModelBase>)GetValue(MediaSourceDependency);
            set => SetValue(MediaSourceDependency, value);
        }

        #endregion

        #region IsLong Dependency Property

        public static readonly DependencyProperty IsLongDependency = DependencyProperty.Register("IsLong", typeof(bool), typeof(MediaElementListUserControl), new PropertyMetadata(false));

        public bool IsLong
        {
            get => (bool)GetValue(IsLongDependency);
            set => SetValue(IsLongDependency, value);
        }

        #endregion

        #region IsProgressBarVisible Dependency Property

        public static readonly DependencyProperty IsProgressBarVisibleDependency = DependencyProperty.Register("IsProgressBarVisible", typeof(bool), typeof(MediaElementListUserControl), new PropertyMetadata(false));

        public bool IsProgressBarVisible
        {
            get => (bool)GetValue(IsProgressBarVisibleDependency);
            set => SetValue(IsProgressBarVisibleDependency, value);
        }

        #endregion

        #region ctor

        public MediaElementListUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Additional methods

        public MediaElementItemUserControl ItemFromGridViewItem(ListViewItem item)
        {
            return item.FindVisualChild<MediaElementItemUserControl>();
        }

        public void SelectFirst()
        {
            if (FirstElement != null)
            {
                MediaElementItemUserControl result = ItemFromGridViewItem(FirstElement);
                result?.FocusGot();
            }
        }

        private void MediaElementListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            NavigableMediaElementViewModelBase dataContext = DataContext as NavigableMediaElementViewModelBase;

            if (dataContext == null)
            {
                // TODO smurancsik: throw new Argument exception
                return;
            }

            dataContext.SelectedMediaElement = e.ClickedItem as MediaElementBase;
            dataContext.NavigateToSelected();
        }
        
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemsContainer == null)
            {
                return;
            }

            object[] arr = e.AddedItems.ToArray();
            object[] delArr = e.RemovedItems.ToArray();

            if (delArr.Length == 0 && FirstElement != null)
            {
                MediaElementItemUserControl item = ItemFromGridViewItem(FirstElement);
                item.FocusLost();
            }
            foreach (object deletedItem in delArr)
            {
                ListViewItem selectedItem = (ListViewItem)itemsContainer.ContainerFromItem(deletedItem);

                MediaElementItemUserControl result = ItemFromGridViewItem(selectedItem);

                result?.FocusLost();
            }

            foreach (object addedItem in arr)
            {
                ListViewItem selectedItem = (ListViewItem)itemsContainer.ContainerFromItem(addedItem);
                MediaElementItemUserControl result = ItemFromGridViewItem(selectedItem);

                result?.FocusGot();
            }
        }

        private void ItemsContainer_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (itemsContainer.Items.Count > 0 && itemsContainer.SelectedItem == null)
            {
                itemsContainer.SelectedIndex = 0;
                SelectFirst();
            }
            else
            {
                if (SelectedElement != null)
                {
                    MediaElementItemUserControl result = ItemFromGridViewItem(SelectedElement);
                    result?.FocusGot();
                }
            }
        }

        public void LoseFocus()
        {
            if (SelectedElement != null)
            {
                MediaElementItemUserControl result = ItemFromGridViewItem(SelectedElement);
                result?.FocusLost();
            }
        }

        #endregion
    }
}
