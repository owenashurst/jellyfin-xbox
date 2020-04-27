using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.ViewModels;

namespace Jellyfin.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaPlaybackView
    {
        #region ctor

        public MediaPlaybackView()
        {
            InitializeComponent();
            (DataContext as MediaPlaybackViewModel).MediaPlayer = mediaPlayerElement;
        }

        #endregion

        #region Additional methods

        /// <summary>
        /// Handles that if the controller B is pressed, stops the playback.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlaybackView_OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Space && playbackMenuView.Visibility == Visibility.Collapsed)
            {
                OpenOsd();
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.Escape && playbackMenuView.Visibility == Visibility.Visible)
            {
                playbackMenuView.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }

            ControllerButtonHandledResult actionResult = (DataContext as MediaPlaybackViewModel).HandleKeyPressed(e.Key);

            if (actionResult == null)
            {
                return;
            }

            e.Handled = true;
            if (actionResult.ShouldOsdOpen)
            {
                OpenOsd(7000);
            }
        }
        
        /// <summary>
        /// Opens the OSD, then triggers a countdown to
        /// hide it automatically after a certain period of time.
        /// </summary>
        /// <param name="interval">The interval to hide it if the user did no interaction to restart the timer.</param>
        private void OpenOsd(int interval = 3500)
        {
            playbackMenuView.Visibility = Visibility.Visible;
            playbackMenuView.pauseButton.Focus(FocusState.Programmatic);
            playbackMenuView.VisibilityChanged(interval);
        }

        /// <summary>
        /// Handles to start playing back the movie passed from the previous frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Movie movie = e.Parameter as Movie;
            (DataContext as MediaPlaybackViewModel).SelectedMediaElement = movie;
            StartPrelude(movie);
        }

        public async Task StartPrelude(Movie movie)
        {
            if (movie.PlaybackInformation == null || !movie.PlaybackInformation.Any())
            {
                return;
            }

            var container = movie.PlaybackInformation.ToList()[0].Container.ToLower();
            if (container.Contains("mkv"))
            {
                // adaptive playback
            }
            else
            {
                // Regular streaming
                StartDirectPlayback(movie.Id);
            }

        }

        /// <summary>
        /// Starts adaptive playback the video with the provided id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task StartAdaptivePlayback(string id)
        {
            string videoUrl =
                Globals.Instance.Host + "/Videos/" + id + "/stream.mov?Static=true&mediaSourceId=" + id + "&deviceId=" + Globals.Instance.SessionInfo.DeviceId + "&api_key=" + Globals.Instance.AccessToken + "&Tag=beb6ef9128431e67c421e4cb890cf84f";

            Uri uri = new Uri(videoUrl);

            mediaPlayerElement.SetMediaPlayer(new MediaPlayer());
            mediaPlayerElement.MediaPlayer.Source = MediaSource.CreateFromUri(uri);
            mediaPlayerElement.MediaPlayer.Play();
            OpenOsd();
        }

        /// <summary>
        /// Starts playing back the video with the provided id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task StartDirectPlayback(string id)
        {
            string videoUrl =
                Globals.Instance.Host + "/Videos/" + id + "/stream.mov?Static=true&mediaSourceId=" + id + "&deviceId=" + Globals.Instance.SessionInfo.DeviceId + "&api_key=" + Globals.Instance.AccessToken + "&Tag=beb6ef9128431e67c421e4cb890cf84f";

            Uri uri = new Uri(videoUrl);

            mediaPlayerElement.SetMediaPlayer(new MediaPlayer());
            mediaPlayerElement.MediaPlayer.Source = MediaSource.CreateFromUri(uri);
            mediaPlayerElement.MediaPlayer.Play();
            OpenOsd();
        }

        #endregion
    }
}
