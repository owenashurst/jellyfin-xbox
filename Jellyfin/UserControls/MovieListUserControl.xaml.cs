using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Jellyfin.Models;
using Jellyfin.ViewModels;

namespace Jellyfin.UserControls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MovieListUserControl
    {
        public MovieListUserControl()
        {
            InitializeComponent();
        }

        private void MovieView_OnLoaded(object sender, RoutedEventArgs e)
        {
            (DataContext as MovieListViewModel).Load();
        }
    }
}
