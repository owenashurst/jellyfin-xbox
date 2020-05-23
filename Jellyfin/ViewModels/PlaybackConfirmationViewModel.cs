using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Windows.System;
using Windows.UI.Core;
using Jellyfin.Logging;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class PlaybackConfirmationViewModel : NavigableMediaElementViewModelBase
    {
        #region Properties

        #region IsShowConfirmation

        private bool _isShowConfirmation = true;

        public bool IsShowConfirmation
        {
            get { return _isShowConfirmation; }
            set
            {
                _isShowConfirmation = value;
                RaisePropertyChanged(nameof(IsShowConfirmation));
            }
        }

        #endregion

        #region NextMediaElement

        private MediaElementBase _nextMediaElement;

        public MediaElementBase NextMediaElement
        {
            get { return _nextMediaElement; }
            set
            {
                _nextMediaElement = value;
                RaisePropertyChanged(nameof(NextMediaElement));
            }
        }

        #endregion

        #region NextAfterMediaElement

        private MediaElementBase _nextAfterMediaElement;

        public MediaElementBase NextAfterMediaElement
        {
            get { return _nextAfterMediaElement; }
            set
            {
                _nextAfterMediaElement = value;
                RaisePropertyChanged(nameof(NextAfterMediaElement));
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

        private Timer _autoPlaybackTimer;

        #region AutoPlayNextTimeLeft

        private int _autoPlayNextTimeLeft;

        public int AutoPlayNextTimeLeft
        {
            get { return _autoPlayNextTimeLeft; }
            set
            {
                _autoPlayNextTimeLeft = value;
                RaisePropertyChanged(nameof(AutoPlayNextTimeLeft));
            }
        }

        #endregion
        
        /// <summary>
        /// Reference for the playback information service.
        /// </summary>
        private readonly IPlaybackInfoService _playbackInfoService;

        /// <summary>
        /// Reference for the log manager.
        /// </summary>
        private readonly ILogManager _logManager;

        #region IsTimerStopped

        private bool _isTimerStopped;

        public bool IsTimerStopped
        {
            get { return _isTimerStopped; }
            set
            {
                _isTimerStopped = value;
                RaisePropertyChanged(nameof(IsTimerStopped));
            }
        }

        #endregion

        #endregion

        #region ctor

        public PlaybackConfirmationViewModel(ITvShowService tvShowService,
            IPlaybackInfoService playbackInfoService, ILogManager logManager)
        {
            _playbackInfoService = playbackInfoService ??
                                   throw new ArgumentNullException(nameof(playbackInfoService));

            _logManager = logManager ??
                          throw new ArgumentNullException(nameof(logManager));

            _autoPlaybackTimer = new Timer();
            _autoPlaybackTimer.AutoReset = true;
            _autoPlaybackTimer.Interval = 1000;
            _autoPlaybackTimer.Elapsed += AutoPlaybackTimerOnElapsed;
        }

        #endregion

        #region Additional methods

        public override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "PlayFromBeginning":
                    PlayFromBeginning();
                    break;
                case "PlayFromPosition":
                    PlayFromPosition();
                    break;
                case "PlayNext":
                    PlayNext();
                    break;
                case "Replay":
                    PlayFromBeginning();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        private async Task PlayFromBeginning()
        {
            if (SelectedMediaElement.PlaybackInformation == null)
            {
                SelectedMediaElement.PlaybackInformation =
                    await _playbackInfoService.GetPlaybackInformation(SelectedMediaElement.Id);
            }

            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                IsPlaybackFromBeginning = true,
                Playlist = PlaybackViewParameters.Playlist
            });
        }

        private async Task PlayFromPosition()
        {
            if (SelectedMediaElement.PlaybackInformation == null)
            {
                SelectedMediaElement.PlaybackInformation =
                    await _playbackInfoService.GetPlaybackInformation(SelectedMediaElement.Id);
            }

            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                IsPlaybackFromBeginning = false,
                Playlist = PlaybackViewParameters.Playlist
            });
        }

        private void PlayNext()
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = NextMediaElement,
                IsPlaybackFromBeginning = true,
                Playlist = PlaybackViewParameters.Playlist
            });
        }

        #endregion

        public bool HandleKeyPressed(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Escape:
                    if (!IsShowConfirmation && NavigationService.GetPreviousPage() == typeof(MediaPlaybackView))
                    {
                        NavigationService.GoBack();
                    }

                    NavigationService.GoBack();
                    return true;
                default:
                    StopTimer();
                    return false;
            }
        }

        public async Task PrepareNextEpisode(PlaybackViewParameterModel vpm)
        {
            IsShowConfirmation = false;

            AutoPlayNextTimeLeft = 20;
            _autoPlaybackTimer.Start();

            SelectedMediaElement = vpm.SelectedMediaElement;
            NextMediaElement = vpm.NextMediaElement;
            NextAfterMediaElement = await GetNextMediaElement((TvShowEpisode)vpm.NextMediaElement);

            _logManager.LogInfo(
                $"Playback finished, Selected Media Element = {SelectedMediaElement}, Next = {NextMediaElement}, Next After = {NextAfterMediaElement}");

            if (NextMediaElement != null)
            {
                NextAfterMediaElement.PlaybackInformation =
                    await _playbackInfoService.GetPlaybackInformation(NextAfterMediaElement.Id);
            }
        }

        private void AutoPlaybackTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            #pragma warning disable CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                AutoPlayNextTimeLeft--;
                if (AutoPlayNextTimeLeft == 0)
                {
                    _autoPlaybackTimer.Stop();
                    PlayNext();
                }
            });
            #pragma warning restore CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public void StopTimer()
        {
            _autoPlaybackTimer.Stop();
            IsTimerStopped = true;
        }

        /// <summary>
        /// Handles the 4 cases of the confirmation navigation from and towards the playback.
        /// Cases:
        /// 1) The playback information is missing,
        ///     or available but less than 2 min., so let it play as is
        /// 2) The playback information is available and more than 2 min, so let the user decide what they want
        /// 3) IsJustFinished is set, display the boxed screen shot layout to the user to let them decide
        /// </summary>
        public async Task PlaybackViewParametersChanged(PlaybackViewParameterModel p)
        {
            _logManager.LogDebug("Playback View Parameters Changed raised, obj = " + PlaybackViewParameters);

            // Does it come from movie / episode chooser?
            if (!p.IsJustFinishedPlaying)
            {
                SelectedMediaElement = p.SelectedMediaElement;
                if (SelectedMediaElement.PlaybackPosition.TotalMinutes > 2 &&
                    SelectedMediaElement.PlaybackRemaining.TotalMinutes < 2)
                {
                    // Let the user decide what they want, aka let the view model
                    // render fully and show the action buttons
                }
                else if (SelectedMediaElement.PlaybackPosition.TotalMinutes <= 2)
                {
                    PlayFromBeginning();
                }
                else if (SelectedMediaElement.PlaybackRemaining.TotalMinutes <= 2)
                {
                    PlayFromBeginning();
                }
            }
            else
            {
                
                // go to the next element on the playlist, then play that from the beginning.
                SelectedMediaElement = PlaybackViewParameters.Playlist[0];
                PlaybackViewParameters.Playlist = PlaybackViewParameters.Playlist.Skip(1).ToArray();

                IsShowConfirmation = true;
            }
        }
    }
}