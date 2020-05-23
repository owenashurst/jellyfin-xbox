using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Models;
using Jellyfin.ViewModels;

namespace Jellyfin.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TvShowDetailView
    {
        public TvShowDetailView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles key press for the UI.
        /// </summary>
        private void MovieDetailView_OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((DataContext as TvShowDetailViewModel).HandleKeyPressed(e.Key))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Retrieves the passed movie parameter from the old view.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MediaElementBase mediaElement = e.Parameter as MediaElementBase;
            if (mediaElement != null)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                (DataContext as TvShowDetailViewModel).GetTvShowDetails(mediaElement);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }
    }
}
