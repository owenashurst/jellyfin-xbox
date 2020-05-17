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
        #region Properties

        public PlaybackConfirmationViewModel _dc
        {
            get => DataContext as PlaybackConfirmationViewModel;
        }

        #endregion

        public PlaybackConfirmationView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            (DataContext as PlaybackConfirmationViewModel).StopTimer();
        }

        
        /// <summary>
        /// Retrieves the playback navigation model, and passes to the view model.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null)
            {
                return;
            }

            PlaybackViewParameterModel m = e.Parameter as PlaybackViewParameterModel;
            _dc.PlaybackViewParameters = m;
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
