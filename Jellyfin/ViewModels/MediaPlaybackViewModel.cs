using System;
using System.Timers;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Jellyfin.Core;
using Jellyfin.Models;

namespace Jellyfin.ViewModels
{
    public class MediaPlaybackViewModel : JellyfinViewModelBase
    {
        #region Properties

        public MediaPlayerElement MediaPlayer { get; set; }

        public Timer OSDUpdateTimer { get; set; }

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

        private Movie _selectedMediaElement;

        public Movie SelectedMediaElement
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

        /// <summary>
        /// Indicates how many seek seconds requested in sum.
        /// </summary>

        #region SeekRequestedSeconds

        private int _seekRequestedSeconds;

        public int SeekRequestedSeconds
        {
            get { return _seekRequestedSeconds; }
            set
            {
                _seekRequestedSeconds = value;

                Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    RaisePropertyChanged(nameof(SeekRequestedSeconds));
                    RaisePropertyChanged(nameof(FormattedSeekRequestedSeconds));
                });
            }
        }

        #endregion

        public string FormattedSeekRequestedSeconds
        {
            get
            {
                if (SeekRequestedSeconds == 0)
                {
                    return string.Empty;
                }
                
                return $" (+{SeekRequestedSeconds}s)";
            }
        }

        #endregion

        #region ctor

        public MediaPlaybackViewModel()
        {
            OSDUpdateTimer = new Timer();
            OSDUpdateTimer.Interval = 1000;
            OSDUpdateTimer.AutoReset = true;
            OSDUpdateTimer.Elapsed += OsdUpdateTimerOnElapsed;
            OSDUpdateTimer.Start();
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

        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Play":
                    Play();
                    break;
                case "Pause":
                    Pause();
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
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        public void Return()
        {
            Pause();
            NavigationService.GoBack();
        }

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
                }
            });
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public void Pause()
        {
            MediaPlayer?.MediaPlayer.Pause();
        }

        public void Play()
        {
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
                    return new ControllerButtonHandledResult { ShouldOsdOpen = true };
                case VirtualKey.GamepadLeftTrigger:
                    Execute("SeekBackward");
                    return new ControllerButtonHandledResult { ShouldOsdOpen = true };
                default:
                    return null;
            }
        }

        #endregion
    }
}