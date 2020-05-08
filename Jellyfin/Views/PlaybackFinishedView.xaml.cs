using Windows.UI.Xaml.Input;
using Jellyfin.ViewModels;

namespace Jellyfin.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaybackFinishedView
    {
        public PlaybackFinishedView()
        {
            InitializeComponent();
        }

        private void PlaybackFinishedView_OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((DataContext as MovieDetailViewModel).HandleKeyPressed(e.Key))
            {
                e.Handled = true;
            }
        }
    }
}
