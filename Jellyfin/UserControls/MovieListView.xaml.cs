using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.ViewModels;

namespace Jellyfin.UserControls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MovieListView
    {
        public MovieListView()
        {
            InitializeComponent();
        }
        
        private void MovieGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            (DataContext as MovieListViewModel).NavigateToMovie(e.ClickedItem as Movie);
        }

        private void MovieView_OnLoaded(object sender, RoutedEventArgs e)
        {
            (DataContext as MovieListViewModel).Load();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView gridView = sender as GridView;

            if (gridView == null)
            {
                return;
            }

            object[] arr = e.AddedItems.ToArray();
            object[] delArr = e.RemovedItems.ToArray();

            foreach (object addedItem in arr)
            {
                GridViewItem selectedItem = (GridViewItem) gridView.ContainerFromItem(addedItem);
                MovieItemUserControl result = selectedItem.FindVisualChild<MovieItemUserControl>();

                result?.StartAnimation();
            }

            foreach (object addedItem in delArr)
            {
                GridViewItem selectedItem = (GridViewItem)gridView.ContainerFromItem(addedItem);
                MovieItemUserControl result = selectedItem.FindVisualChild<MovieItemUserControl>();

                result?.EndAnimation();
            }
        }
    }
}
