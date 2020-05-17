using Windows.UI.Xaml;
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            (DataContext as MovieListViewModel).Load();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
