using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
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
            Loaded += OnLoaded;
        }
        
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //_dc.PlaybackViewParametersChanged(_dc.PlaybackViewParameters);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _dc.StopTimer();
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
            if (m.IsInvalidated)
            {
                return;
            }

            m.IsInvalidated = true;
            _dc.PlaybackViewParameters = m;

            Task.Delay(0).ContinueWith((task) =>
            {
                Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        _dc.PlaybackViewParametersChanged(m);
                    });
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
