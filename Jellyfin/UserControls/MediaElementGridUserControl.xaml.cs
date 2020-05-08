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
    public sealed partial class MediaElementGridUserControl
    {
        #region ItemsSource Dependency Property

        public static readonly DependencyProperty MediaSourceDependency =
            DependencyProperty.Register("MediaSource", typeof(ObservableCollection<ModelBase>),
                typeof(MediaElementGridUserControl), new PropertyMetadata(null));

        public ObservableCollection<ModelBase> MediaSource
        {
            get => (ObservableCollection<ModelBase>) GetValue(MediaSourceDependency);
            set => SetValue(MediaSourceDependency, value);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates the first element of the grid.
        /// </summary>
        public GridViewItem FirstElement
        {
            get => (GridViewItem)itemsContainer.ContainerFromIndex(0);
        }

        #endregion

        #region ctor

        public MediaElementGridUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Additional methods

        /// <summary>
        /// Creates a the item user control of the grid view item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public MediaElementItemUserControl ItemFromGridViewItem(GridViewItem item)
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

        private void MediaElementGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            MediaElementListViewModelBase dataContext = DataContext as MediaElementListViewModelBase;

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
                GridViewItem selectedItem = (GridViewItem) itemsContainer.ContainerFromItem(deletedItem);

                MediaElementItemUserControl result = ItemFromGridViewItem(selectedItem);

                result?.FocusLost();
            }

            foreach (object addedItem in arr)
            {
                GridViewItem selectedItem = (GridViewItem) itemsContainer.ContainerFromItem(addedItem);
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
        }

        #endregion
    }
}
