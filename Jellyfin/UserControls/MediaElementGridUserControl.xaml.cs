using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.ViewModels;

namespace Jellyfin.UserControls
{
    public sealed partial class MediaElementGridUserControl
    {
        #region ItemsSource Dependency Property
        
        public static readonly DependencyProperty MediaSourceDependency = DependencyProperty.Register("MediaSource", typeof(ObservableCollection<ModelBase>), typeof(MediaElementGridUserControl), new PropertyMetadata(null));

        public ObservableCollection<ModelBase> MediaSource
        {
            get => (ObservableCollection<ModelBase>)GetValue(MediaSourceDependency);
            set => SetValue(MediaSourceDependency, value);
        }

        #endregion

        public MediaElementGridUserControl()
        {
            InitializeComponent();
        }

        public void SelectFirst()
        {
            GridViewItem selectedItem = (GridViewItem)itemsContainer.ContainerFromIndex(0);

            if (selectedItem != null)
            {
                MovieItemUserControl result = selectedItem.FindVisualChild<MovieItemUserControl>();

                result?.StartAnimation();
            }
        }

        private void MovieGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            (DataContext as MovieListViewModel).NavigateToMovie(e.ClickedItem as Movie);
        }
        
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemsContainer == null)
            {
                return;
            }

            object[] arr = e.AddedItems.ToArray();
            object[] delArr = e.RemovedItems.ToArray();

            foreach (object addedItem in arr)
            {
                GridViewItem selectedItem = (GridViewItem)itemsContainer.ContainerFromItem(addedItem);
                MovieItemUserControl result = selectedItem.FindVisualChild<MovieItemUserControl>();

                result?.StartAnimation();
            }

            foreach (object addedItem in delArr)
            {
                GridViewItem selectedItem = (GridViewItem)itemsContainer.ContainerFromItem(addedItem);
                MovieItemUserControl result = selectedItem.FindVisualChild<MovieItemUserControl>();

                result?.EndAnimation();
            }
        }

        private void ItemsContainer_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (itemsContainer.Items.Count > 0 && itemsContainer.SelectedItem == null)
            {
                SelectFirst();
            }
        }
    }
}
