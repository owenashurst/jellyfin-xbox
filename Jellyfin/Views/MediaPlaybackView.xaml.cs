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
using Jellyfin.Logging;
using Jellyfin.Models;
using Jellyfin.ViewModels;
using Newtonsoft.Json;
using Unity;

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

        private ILogManager _logManager;

        #endregion

        #region ctor

        public MediaPlaybackView()
        {
            InitializeComponent();

            _logManager = Globals.Instance.Container.Resolve<ILogManager>();
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
        [LogMethod]
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

            playbackMenuView.playButton.Focus(FocusState.Programmatic);
            playbackMenuView.VisibilityChanged(interval);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (mediaPlayerElement.MediaPlayer == null)
            {
                _logManager.LogWarn("Somehow the media player is null. Grab a programmer!");
                return;
            }

            mediaPlayerElement.MediaPlayer.Pause();

            //base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Handles to start playing back the media element passed from the previous frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PlaybackViewParameterModel playbackViewParameterModel = e.Parameter as PlaybackViewParameterModel;

            _dataContext.PlaybackViewParameters = playbackViewParameterModel;
            _dataContext.SelectedMediaElement = playbackViewParameterModel.SelectedMediaElement;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Prelude(playbackViewParameterModel);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            base.OnNavigatedTo(e);
        }

        public async Task Prelude(PlaybackViewParameterModel playbackViewParameterModel)
        {
            MediaElementBase mediaElement = playbackViewParameterModel?.SelectedMediaElement;
            if (mediaElement?.PlaybackInformation == null || !mediaElement.PlaybackInformation.Any())
            {
                return;
            }

            _dataContext.IsLoading = true;
            
            MediaElementPlaybackSource playbackInformation = mediaElement.PlaybackInformation.ToList()[0];
            if (!string.IsNullOrEmpty(playbackInformation.TranscodingUrl))
            {
                _logManager.LogDebug(
                    $"{mediaElement} playback: AMS mode. Transcoding URL: {playbackInformation.TranscodingUrl}");

                AdaptiveMediaSource ams;

                Uri uri = new Uri(Globals.Instance.Host + playbackInformation.TranscodingUrl);
                AdaptiveMediaSourceCreationResult result = await AdaptiveMediaSource.CreateFromUriAsync(uri);

                if (result.Status == AdaptiveMediaSourceCreationStatus.Success)
                {
                    ams = result.MediaSource;
                    mediaPlayerElement.SetMediaPlayer(new MediaPlayer());
                    mediaPlayerElement.MediaPlayer.Source = MediaSource.CreateFromAdaptiveMediaSource(ams);

                    SubscribeToEvents(mediaPlayerElement.MediaPlayer, playbackViewParameterModel);

                    mediaPlayerElement.MediaPlayer.Play();
                    OpenOsd();

                    ams.InitialBitrate = ams.AvailableBitrates.Max();
                }

                _dataContext.PlaybackMode = "Transcoding";
            }
            else
            {
                _logManager.LogDebug($"{mediaElement} playback: direct play.");

                // Regular streaming
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                StartDirectPlayback(playbackViewParameterModel);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                _dataContext.PlaybackMode = "DirectStream";
            }

        }

        private void SubscribeToEvents(MediaPlayer mediaPlayer, PlaybackViewParameterModel vm)
        {
            mediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
            mediaPlayer.PlaybackSession.BufferingStarted += (sender, args) =>
            {
                LogStatus("Buffered started");
            };

            mediaPlayer.PlaybackSession.BufferingEnded += (sender, args) =>
            {
                LogStatus("Buffered ended");
            };

            mediaPlayer.PlaybackSession.BufferingProgressChanged += (sender, args) =>
            {
                LogStatus("Buffered progress changed");
            };

            mediaPlayer.PlaybackSession.BufferedRangesChanged += (sender, args) =>
            {
                LogStatus("Buffered ranges changed");
            };

            mediaPlayer.MediaEnded += MediaPlayerOnMediaEnded;

            if (!vm.IsPlaybackFromBeginning)
            {
                mediaPlayer.PlaybackSession.NaturalDurationChanged += PlaybackSessionOnNaturalDurationChanged;
            }
        }

        private void LogStatus(string status)
        {
            // Enable only for debugging. Makes playback responsiveness laggy
            //Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            //{
            //    try
            //    {
            //        var quick = new
            //        {
            //            NaturalDuration = mediaPlayerElement.MediaPlayer.PlaybackSession.NaturalDuration,
            //            PlaybackRate = mediaPlayerElement.MediaPlayer.PlaybackSession.PlaybackRate,
            //            BufferingProgress = mediaPlayerElement.MediaPlayer.PlaybackSession.BufferingProgress,
            //            CanPause = mediaPlayerElement.MediaPlayer.PlaybackSession.CanPause,
            //            CanSeek = mediaPlayerElement.MediaPlayer.PlaybackSession.CanSeek,
            //            DownloadProgress = mediaPlayerElement.MediaPlayer.PlaybackSession.DownloadProgress,
            //            IsMirroring = mediaPlayerElement.MediaPlayer.PlaybackSession.IsMirroring,
            //            NaturalVideoHeight = mediaPlayerElement.MediaPlayer.PlaybackSession.NaturalVideoHeight,
            //            NaturalVideoWidth = mediaPlayerElement.MediaPlayer.PlaybackSession.NaturalVideoWidth,
            //            PlaybackState = mediaPlayerElement.MediaPlayer.PlaybackSession.PlaybackState.ToString(),
            //            Position = mediaPlayerElement.MediaPlayer.PlaybackSession.Position,
            //            IsPlaying = String.Join(", ",
            //                mediaPlayerElement.MediaPlayer.PlaybackSession.GetBufferedRanges())
            //        };

            //        _logManager.LogDebug($"{status}: {JsonConvert.SerializeObject(quick)}");
            //    } catch (Exception)
            //    {

            //    }
            //});
        } 

        /// <summary>
        /// Gets triggered when the media playback is ended.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MediaPlayerOnMediaEnded(MediaPlayer sender, object args)
        {
            LogStatus("Media ended");

            #pragma warning disable CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                _dataContext.PromptNextEpisode();
            });
            #pragma warning restore CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            LogStatus("Playback state changed");

            #pragma warning disable CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                _logManager.LogDebug("Playback state changed: " + sender.PlaybackState);
                _dataContext.IsLoading = sender.PlaybackState == MediaPlaybackState.Buffering;
            });
            #pragma warning restore CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// Starts playing back the video with the provided id.
        /// </summary>
        /// <param name="playbackViewParameterModel">The playback view parameters.</param>
        /// <returns></returns>
        public async Task StartDirectPlayback(PlaybackViewParameterModel playbackViewParameterModel)
        {
            MediaElementBase mediaElement = playbackViewParameterModel.SelectedMediaElement;
            string id = mediaElement.Id;

            string videoUrl =
                $"{Globals.Instance.Host}/Videos/{id}/stream.mov?Static=true&mediaSourceId={id}&deviceId={Globals.Instance.SessionInfo.DeviceId}&api_key={Globals.Instance.AccessToken}&Tag=beb6ef9128431e67c421e4cb890cf84f";

            Uri uri = new Uri(videoUrl);
            mediaPlayerElement.MediaPlayer.Source = MediaSource.CreateFromUri(uri);

            SubscribeToEvents(mediaPlayerElement.MediaPlayer, playbackViewParameterModel);

            mediaPlayerElement.MediaPlayer.Play();
            OpenOsd();
        }

        /// <summary>
        /// Sets the playback position, then unsubscribes from the event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PlaybackSessionOnNaturalDurationChanged(MediaPlaybackSession sender, object args)
        {
            LogStatus("Natural Duration changed");

            #pragma warning disable CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                MediaPlaybackSession session = mediaPlayerElement.MediaPlayer.PlaybackSession;
                session.Position = _dataContext.SelectedMediaElement.PlaybackPosition;
                session.NaturalDurationChanged -= PlaybackSessionOnNaturalDurationChanged;
            });
            #pragma warning restore CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        
        #endregion
    }
}
