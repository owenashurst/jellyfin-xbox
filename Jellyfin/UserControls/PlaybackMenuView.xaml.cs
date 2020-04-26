using System.Timers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Jellyfin.Core;

namespace Jellyfin.UserControls
{
    public sealed partial class PlaybackMenuView
    {
        #region Additional methods

        private Timer AutoHideTimer { get; set; }

        #endregion

        #region ctor

        public PlaybackMenuView()
        {
            InitializeComponent();
            AutoHideTimer = new Timer();
            AutoHideTimer.Interval = 3500;
            AutoHideTimer.AutoReset = false;
            AutoHideTimer.Elapsed += AutoHideTimerOnElapsed;
        }

        #endregion

        #region Additional methods

        private void AutoHideTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                Visibility = Visibility.Collapsed;
            });
        }

        public void VisibilityChanged(int interval = 3500)
        {
            AutoHideTimer.Interval = interval;
            AutoHideTimer.Start();
        }

        #endregion
    }
}
