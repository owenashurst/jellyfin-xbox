using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;
using Unity;

namespace Jellyfin.ViewModels
{
    public class MediaPlaybackViewModel : JellyfinViewModelBase
    {
        #region Properties

        public MediaPlayerElement MediaPlayer { get; set; }

        public Timer OSDUpdateTimer { get; set; }

        /// <summary>
        /// Timer for reporting playback status.
        /// </summary>
        public Timer ReportPlaybackStatusTimer { get; set; }

        public bool WasPlaybackPopupShown { get; set; }

        #region RemainingTimeLeft

        private TimeSpan _remainingTimeLeft;

        public TimeSpan RemainingTimeLeft
        {
            get { return _remainingTimeLeft; }
            set
            {
                _remainingTimeLeft = value;
                RaisePropertyChanged(nameof(RemainingTimeLeft));
            }
        }

        #endregion

        #region SelectedMediaElement

        private MediaElementBase _selectedMediaElement;

        public MediaElementBase SelectedMediaElement
        {
            get { return _selectedMediaElement; }
            set
            {
                _selectedMediaElement = value;
                RaisePropertyChanged(nameof(SelectedMediaElement));
            }
        }

        #endregion

        private Timer SeekRequestTimer;

        /// <summary>
        /// Thread locking for seek request timer configuration
        /// </summary>
        private object padlock = new object();

        #region IsOsdKeepOnScreen

        private bool _isOsdKeepOnScreen;

        /// <summary>
        /// Indicates whether the OSD should be kept on screen.
        /// </summary>
        public bool IsOsdKeepOnScreen
        {
            get { return _isOsdKeepOnScreen; }
            set
            {
                _isOsdKeepOnScreen = value;
                RaisePropertyChanged(nameof(IsOsdKeepOnScreen));
            }
        }

        #endregion

        #region SeekRequestedSeconds

        private int _seekRequestedSeconds;

        /// <summary>
        /// Indicates how many seek seconds requested in sum.
        /// </summary>
        public int SeekRequestedSeconds
        {
            get { return _seekRequestedSeconds; }
            set
            {
                _seekRequestedSeconds = value;

                #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    RaisePropertyChanged(nameof(SeekRequestedSeconds));
                    RaisePropertyChanged(nameof(FormattedSeekRequestedSeconds));
                });
                #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        #endregion

        #region FormattedSeekRequestedSeconds

        public string FormattedSeekRequestedSeconds
        {
            get
            {
                if (SeekRequestedSeconds == 0)
                {
                    return string.Empty;
                }

                if (SeekRequestedSeconds < 0)
                {
                    if (SeekRequestedSeconds < -90)
                    {
                        int minutes = SeekRequestedSeconds / 60;
                        int seconds = Math.Abs(SeekRequestedSeconds % 60);
                        return $" ({minutes}min {seconds}s)";
                    }

                    return $" ({SeekRequestedSeconds}s)";
                }

                if (SeekRequestedSeconds > 90)
                {
                    int minutes = SeekRequestedSeconds / 60;
                    int seconds = SeekRequestedSeconds % 60;
                    return $" (+{minutes}min {seconds}s)";
                }

                return $" (+{SeekRequestedSeconds}s)";
            }
        }

        #endregion
        
        #region PlaybackMode

        private string _playbackMode;

        /// <summary>
        /// Indicates the playback mode: is it transcoding or direct stream.
        /// </summary>
        public string PlaybackMode
        {
            get { return _playbackMode; }
            set
            {
                _playbackMode = value;
                RaisePropertyChanged(nameof(PlaybackMode));
            }
        }

        #endregion

        /// <summary>
        /// The service for reporting current playback.
        /// </summary>
        private readonly IReportProgressService _reportProgressService;

        #region IsPlaying

        private bool _isPlaying;

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                RaisePropertyChanged(nameof(IsPlaying));
            }
        }

        #endregion

        #region PlaybackViewParameters

        private PlaybackViewParameterModel _playbackViewParameters;

        public PlaybackViewParameterModel PlaybackViewParameters
        {
            get { return _playbackViewParameters; }
            set
            {
                if (_playbackViewParameters != value)
                {
                    _playbackViewParameters = value;
                    RaisePropertyChanged(nameof(PlaybackViewParameters));
                }
            }
        }

        #endregion

        #endregion

        #region ctor

        public MediaPlaybackViewModel(IReportProgressService reportProgressService)
        {
            OSDUpdateTimer = new Timer();
            OSDUpdateTimer.Interval = 1000;
            OSDUpdateTimer.AutoReset = true;
            OSDUpdateTimer.Elapsed += OsdUpdateTimerOnElapsed;
            OSDUpdateTimer.Start();

            _reportProgressService = reportProgressService ??
                throw new ArgumentNullException(nameof(reportProgressService));

            ReportPlaybackStatusTimer = new Timer();
            ReportPlaybackStatusTimer.Interval = 60 * 1000;
            ReportPlaybackStatusTimer.AutoReset = true;
            ReportPlaybackStatusTimer.Elapsed += ReportPlaybackStatusTimer_Elapsed;
            ReportPlaybackStatusTimer.Start();

            IsPlaying = true;
        }
        
        #endregion

        #region Additional methods

        public override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Play":
                    Play();
                    break;
                case "Pause":
                    Pause();
                    break;
                case "PlayPause":
                    PlayPause();
                    break;
                case "Return":
                    Return();
                    break;
                case "SeekForward":
                    SeekRequest(30);
                    break;
                case "SeekBackward":
                    SeekRequest(-30);
                    break;
                case "StepBackward":
                    StepBackward();
                    break;
                case "StepForward":
                    // TODO for TV shows
                    break;
                case "FastBackward":
                    FastBackward();
                    break;
                case "FastForward":
                    FastForward();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        public void Return()
        {
            Pause();
            NavigationService.GoBack();
            NavigationService.GoBack();
        }

        public async Task PromptNextEpisode()
        {
            if (PlaybackViewParameters.Playlist.Any())
            {
                IUnityContainer container = Globals.Instance.Container;
                PlaybackConfirmationViewModel confirmationVm =
                    container.Resolve<PlaybackConfirmationViewModel>();

                var pvpm =  new PlaybackViewParameterModel
                {
                    SelectedMediaElement = SelectedMediaElement,
                    IsJustFinishedPlaying = true,
                    Playlist = PlaybackViewParameters.Playlist,
                };

                NavigationService.GoBack();
                confirmationVm.PlaybackViewParametersChanged(pvpm);
            }
            else
            {
                Return();
            }
        }

        #region Seek implementation

        public void SeekRequest(int seconds)
        {
            lock (padlock)
            {
                if (SeekRequestTimer == null)
                {
                    SeekRequestedSeconds = seconds;

                    SeekRequestTimer = new Timer();
                    SeekRequestTimer.Interval = 1000;
                    SeekRequestTimer.Elapsed += SeekRequestTimerOnElapsed;
                    SeekRequestTimer.AutoReset = false;
                    SeekRequestTimer.Start();
                }
                else
                {
                    SeekRequestedSeconds += seconds;

                    SeekRequestTimer.Interval = 1000;
                    SeekRequestTimer.Start();
                }
            }
        }

        private void SeekRequestTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            SeekRequestTimer.Stop();
            Seek(SeekRequestedSeconds);
            SeekRequestedSeconds = 0;

            SeekRequestTimer.Elapsed -= SeekRequestTimerOnElapsed;
            SeekRequestTimer = null;
        }

        public void Seek(int seconds)
        {
            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (MediaPlayer != null)
                {
                    MediaPlayer.MediaPlayer.PlaybackSession.Position =
                        MediaPlayer.MediaPlayer.PlaybackSession.Position + TimeSpan.FromSeconds(seconds);

                    if (PlaybackMode == "DirectStream")
                    {
                        IsLoading = false;
                    }
                }
            });
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// Rewinds to the beginning. 
        /// </summary>
        public void StepBackward()
        {
            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (MediaPlayer != null)
                {
                    MediaPlayer.MediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
                }
            });
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public void FastBackward()
        {
            SeekRequest(-2 * 60);
        }

        public void FastForward()
        {
            SeekRequest(2 * 60);
        }

        #endregion

        public void PlayPause()
        {
            if (IsPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }


        public void Pause()
        {
            IsPlaying = false;
            MediaPlayer?.MediaPlayer.Pause();
        }

        public void Play()
        {
            IsPlaying = true;
            MediaPlayer?.MediaPlayer.Play();
        }

        public ControllerButtonHandledResult HandleKeyPressed(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Escape:
                    Return();
                    return new ControllerButtonHandledResult();
                case VirtualKey.GamepadRightTrigger:
                    Execute("SeekForward");
                    return new ControllerButtonHandledResult
                    {
                        ShouldOsdOpen = true,
                        ShouldStartLoading = true
                    };
                case VirtualKey.GamepadLeftTrigger:
                    Execute("SeekBackward");
                    return new ControllerButtonHandledResult
                    {
                        ShouldOsdOpen = true,
                        ShouldStartLoading = true
                    };
                default:
                    return null;
            }
        }

        /// <summary>
        /// Reports back the current playback poisiton every minute.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportPlaybackStatusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (SelectedMediaElement == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(PlaybackMode))
            {
                return;
            }
            
            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                if (MediaPlayer?.MediaPlayer?.PlaybackSession?.Position == null)
                {
                    return;
                }

                _reportProgressService.Report(SelectedMediaElement.Id, PlaybackMode,
                    MediaPlayer.MediaPlayer.PlaybackSession.Position);
            });
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        private void OsdUpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
#pragma warning disable CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MediaPlayer != null && MediaPlayer.MediaPlayer != null)
                {
                    MediaPlayer mp = MediaPlayer.MediaPlayer;
                    RemainingTimeLeft = mp.NaturalDuration - mp.PlaybackSession.Position;

                    RaisePropertyChanged(nameof(MediaPlayer));
                }
                else
                {
                    RemainingTimeLeft = TimeSpan.Zero;
                }
            });
#pragma warning restore CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        #endregion
    }
}