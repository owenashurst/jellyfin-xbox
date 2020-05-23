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

        private void TvShowListView_OnLoaded(object sender, RoutedEventArgs e)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            (DataContext as TvShowListViewModel).Load();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
