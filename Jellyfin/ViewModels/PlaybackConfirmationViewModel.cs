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

        #region IsTimerVisible

        private bool _isTimerVisible;

        public bool IsTimerVisible
        {
            get { return _isTimerVisible; }
            set
            {
                _isTimerVisible = value;
                RaisePropertyChanged(nameof(IsTimerVisible));
            }
        }

        #endregion

        #endregion

        #region ctor

        public PlaybackConfirmationViewModel(IPlaybackInfoService playbackInfoService, IPersonalizeService personalizeService,
            ILogManager logManager) : base(personalizeService, logManager)
        {
            _playbackInfoService = playbackInfoService ??
                                   throw new ArgumentNullException(nameof(playbackInfoService));

            _autoPlaybackTimer = new Timer();
            _autoPlaybackTimer.AutoReset = true;
            _autoPlaybackTimer.Interval = 1000;
            IsTimerVisible = true;
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

        private async Task PlayNext()
        {
            if (NextMediaElement.PlaybackInformation == null)
            {
                NextMediaElement.PlaybackInformation =
                    await _playbackInfoService.GetPlaybackInformation(NextMediaElement.Id);
            }

            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = NextMediaElement,
                IsPlaybackFromBeginning = true,
                Playlist = PlaybackViewParameters.Playlist
            });
        }

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
            IsTimerVisible = false;
        }

        /// <summary>
        /// Handles the 4 cases of the confirmation navigation from and towards the playback.
        /// Cases:
        /// 1) The playback information is missing,
        ///     or available but less than 2 min., so let it play as is
        /// 2) The playback information is available and more than 2 min, so let the user decide what they want
        /// 3) IsJustFinished is set, display the boxed screen shot layout to the user to let them decide
        /// </summary>
        [LogMethod]
        public async Task PlaybackViewParametersChanged(PlaybackViewParameterModel p)
        {
            _logManager.LogDebug($"Playback View Parameters Changed raised, obj = {PlaybackViewParameters}");

            // Does it come from movie / episode chooser?
            if (!p.IsJustFinishedPlaying)
            {
                _logManager.LogDebug($"{p.SelectedMediaElement}, It comes from movie/epi chooser.");
                IsShowConfirmation = false;

                SelectedMediaElement = p.SelectedMediaElement;
                if (SelectedMediaElement.PlaybackPosition.TotalMinutes > 2 &&
                    SelectedMediaElement.PlaybackRemaining.TotalMinutes < 2)
                {
                    // Let the user decide what they want, aka let the view model
                    // render fully and show the action buttons
                }
                else if (SelectedMediaElement.PlaybackPosition.TotalMinutes <= 2)
                {
                    _logManager.LogDebug($"{p.SelectedMediaElement}, playback position total less than 2min.");
                    PlayFromBeginning();
                }
                else if (SelectedMediaElement.PlaybackRemaining.TotalMinutes <= 2)
                {
                    _logManager.LogDebug($"{p.SelectedMediaElement}, playback remaining total less than 2min.");
                    PlayFromBeginning();
                }
            }
            else
            {
                
                // go to the next element on the playlist, then play that from the beginning.
                SelectedMediaElement = PlaybackViewParameters.SelectedMediaElement;
                if (PlaybackViewParameters.Playlist.Length >= 2)
                {
                    _logManager.LogDebug(
                        $"{p.SelectedMediaElement}, go to the next element on the playlist, then play that from the beginning.");

                    NextMediaElement = PlaybackViewParameters.Playlist[0];
                    PlaybackViewParameters.Playlist = PlaybackViewParameters.Playlist.Skip(1).ToArray();

                    IsShowConfirmation = true;

                    AutoPlayNextTimeLeft = 20;
                    _autoPlaybackTimer.Start();
                    IsTimerVisible = true;
                }
                else
                {
                    _logManager.LogDebug("No next element, navigating back twice.");

                    // As it comes from the playback, we should navigate back two times
                    NavigationService.GoBack();
                    NavigationService.GoBack();
                }
            }
        }

        #endregion

    }
}