using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Models;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var vpm = e.Parameter as PlaybackViewParameterModel;

            if (vpm != null)
            {
                (DataContext as PlaybackFinishedViewModel).Update(vpm);
            }
        }

        private void PlaybackFinishedView_OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((DataContext as PlaybackFinishedViewModel).HandleKeyPressed(e.Key))
            {
                e.Handled = true;
            }
        }
    }
}
