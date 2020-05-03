using System;
using System.Timers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Jellyfin.Core;
using Jellyfin.ViewModels;

namespace Jellyfin.UserControls
{
    public sealed partial class PlaybackMenuUserControl
    {
        #region Additional methods

        private Timer AutoHideTimer { get; set; }

        private int initialInterval = 3500;

        #endregion

        #region ctor

        public PlaybackMenuUserControl()
        {
            InitializeComponent();
            AutoHideTimer = new Timer();
            AutoHideTimer.Interval = 3500;
            AutoHideTimer.AutoReset = false;
            AutoHideTimer.Elapsed += AutoHideTimerOnElapsed;
        }

        #endregion

        #region Additional methods

        private async void AutoHideTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            await Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if ((DataContext as MediaPlaybackViewModel).IsOsdKeepOnScreen)
                {
                    (sender as Timer).Stop();
                    (sender as Timer).Start();
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }
            });
        }

        public void VisibilityChanged(int interval = 3500)
        {
            AutoHideTimer.Interval = interval;
            initialInterval = interval;

            AutoHideTimer.Start();
        }

        #endregion

        private void StayAwake(object sender, RoutedEventArgs e)
        {
            AutoHideTimer.Stop();
            AutoHideTimer.Start();
        }
    }
}
