using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.System;
using Windows.UI.Core;
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
        #region Properties

        private MediaPlaybackViewModel _dataContext
        {
            get => (DataContext as MediaPlaybackViewModel);
        }

        #endregion

        #region ctor

        public MediaPlaybackView()
        {
            InitializeComponent();

            _dataContext.MediaPlayer = mediaPlayerElement;
        }

        #endregion

        #region Additional methods

        /// <summary>
        /// Handles generic controller actions.
        /// * if the controller B is pressed, stops the playback.
        /// * Seeking with the trigger buttons
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

            ControllerButtonHandledResult actionResult = _dataContext.HandleKeyPressed(e.Key);

            if (actionResult == null)
            {
                return;
            }

            e.Handled = true;
            if (actionResult.ShouldOsdOpen)
            {
                OpenOsd(7000);
            }

            if (actionResult.ShouldStartLoading)
            {
                _dataContext.IsLoading = true;
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
            PlaybackViewParameters playbackViewParameters = e.Parameter as PlaybackViewParameters;
            
            _dataContext.SelectedMediaElement = playbackViewParameters.SelectedMovie;
            _dataContext.IsPlaybackConfirmationDisplayedBefore = !playbackViewParameters.IsPlaybackFromBeginning;

            StartPrelude(playbackViewParameters);
        }

        public async Task StartPrelude(PlaybackViewParameters playbackViewParameters)
        {
            Movie movie = playbackViewParameters?.SelectedMovie;
            if (movie?.PlaybackInformation == null || !movie.PlaybackInformation.Any())
            {
                return;
            }

            MediaElementPlaybackSource playbackInformation = movie.PlaybackInformation.ToList()[0];
            if (!string.IsNullOrEmpty(playbackInformation.TranscodingUrl))
            {
                AdaptiveMediaSource ams;

                Uri uri = new Uri(Globals.Instance.Host + playbackInformation.TranscodingUrl);
                AdaptiveMediaSourceCreationResult result = await AdaptiveMediaSource.CreateFromUriAsync(uri);

                if (result.Status == AdaptiveMediaSourceCreationStatus.Success)
                {
                    ams = result.MediaSource;
                    mediaPlayerElement.SetMediaPlayer(new MediaPlayer());
                    mediaPlayerElement.MediaPlayer.Source = MediaSource.CreateFromAdaptiveMediaSource(ams);
                    
                    mediaPlayerElement.MediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
                    
                    mediaPlayerElement.MediaPlayer.Play();
                    
                    ams.InitialBitrate = ams.AvailableBitrates.Max<uint>();
                }

                _dataContext.PlaybackMode = "Transcoding";
            }
            else
            {
                // Regular streaming
                StartDirectPlayback(playbackViewParameters);

                _dataContext.PlaybackMode = "DirectStream";
            }

        }
        
        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                _dataContext.IsLoading = sender.PlaybackState == MediaPlaybackState.Buffering;
            });
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// Starts playing back the video with the provided id.
        /// </summary>
        /// <param name="playbackViewParameters">The playback view parameters.</param>
        /// <returns></returns>
        public async Task StartDirectPlayback(PlaybackViewParameters playbackViewParameters)
        {
            Movie movie = playbackViewParameters.SelectedMovie;
            string id = movie.Id;

            string videoUrl =
                $"{Globals.Instance.Host}/Videos/{id}/stream.mov?Static=true&mediaSourceId={id}&deviceId={Globals.Instance.SessionInfo.DeviceId}&api_key={Globals.Instance.AccessToken}&Tag=beb6ef9128431e67c421e4cb890cf84f";

            Uri uri = new Uri(videoUrl);

            mediaPlayerElement.SetMediaPlayer(new MediaPlayer());
            mediaPlayerElement.MediaPlayer.Source = MediaSource.CreateFromUri(uri);
            
            mediaPlayerElement.MediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
            
            if (!playbackViewParameters.IsPlaybackFromBeginning)
            {
                mediaPlayerElement.MediaPlayer.PlaybackSession.NaturalDurationChanged += PlaybackSessionOnNaturalDurationChanged;
            }

            mediaPlayerElement.MediaPlayer.Play();
            OpenOsd();
        }

        private void PlaybackSessionOnNaturalDurationChanged(MediaPlaybackSession sender, object args)
        {
            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                var session = mediaPlayerElement.MediaPlayer.PlaybackSession;
                session.Position = _dataContext.SelectedMediaElement.PlaybackPosition;

                session.NaturalDurationChanged -= PlaybackSessionOnNaturalDurationChanged;
            });
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        
        #endregion
    }
}
