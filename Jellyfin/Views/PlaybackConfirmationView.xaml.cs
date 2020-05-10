using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Models;
using Jellyfin.ViewModels;

namespace Jellyfin.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaybackConfirmationView
    {
        public PlaybackConfirmationView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            (DataContext as PlaybackConfirmationViewModel).StopTimer();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null)
            {
                // If it was null, it means it comes from the medis playback view, and it's already been configured.
                // TODO smurancsik: to channel that call to there too
                return;
            }

            MediaElementBase selectedMediaElement = e.Parameter as MediaElementBase;
            MediaElementBase nextMediaElement = null;
            if (selectedMediaElement == null)
            {
                PlaybackViewParameterModel m = e.Parameter as PlaybackViewParameterModel;
                selectedMediaElement = m.SelectedMediaElement;
                nextMediaElement = m.NextMediaElement;
            }

            (DataContext as PlaybackConfirmationViewModel).SelectedMediaElement = selectedMediaElement;
            (DataContext as PlaybackConfirmationViewModel).NextMediaElement = nextMediaElement;
        }

        private void PlaybackConfirmationView_OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((DataContext as PlaybackConfirmationViewModel).HandleKeyPressed(e.Key))
            {
                e.Handled = true;
            }
        }
    }
}
