using Windows.UI.Xaml;
using Jellyfin.ViewModels;

namespace Jellyfin.UserControls
{
    public sealed partial class TvShowListUserControl
    {
        public TvShowListUserControl()
        {
            InitializeComponent();
        }

        private void TvShowView_OnLoaded(object sender, RoutedEventArgs e)
        {
            (DataContext as MovieListViewModel).Load();
        }
    }
}
