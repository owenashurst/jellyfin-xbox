using Windows.UI.Core;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Logging;
using Jellyfin.Models;
using Jellyfin.ViewModels;
using Newtonsoft.Json;

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
            _dc.StopTimer();
        }

        
        /// <summary>
        /// Retrieves the playback navigation model, and passes to the view model.
        /// </summary>
        /// <param name="e"></param>
        [LogMethod]
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null)
            {
                return;
            }

            PlaybackViewParameterModel m = e.Parameter as PlaybackViewParameterModel;
            if (m.IsInvalidated)
            {
                Globals.Instance.LogManager.LogDebug(
                    $"Playback Confirmation View: View Parameter model is invalidated, not handling navigation logic. e.Parameter = {m}");
                return;
            }

            m.IsInvalidated = true;
            _dc.PlaybackViewParameters = m;

            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                _dc.PlaybackViewParametersChanged(m);
            });
        }

        private void PlaybackConfirmationView_OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (_dc.HandleKeyPressed(e.Key))
            {
                e.Handled = true;
            }
        }
    }
}
