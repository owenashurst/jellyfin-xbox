using System;
using System.Text;
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
        /// Reference for the tv show service, to retrieve the next element.
        /// </summary>
        private readonly ITvShowService _tvShowService;

        /// <summary>
        /// Reference for the playback information service.
        /// </summary>
        private readonly IPlaybackInfoService _playbackInfoService;

        /// <summary>
        /// Reference for the log manager.
        /// </summary>
        private readonly ILogManager _logManager;

        #endregion

        #region ctor

        public PlaybackConfirmationViewModel(ITvShowService tvShowService,
            IPlaybackInfoService playbackInfoService, ILogManager logManager)
        {
            _tvShowService = tvShowService ??
                             throw new ArgumentNullException(nameof(tvShowService));

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
                    PlayFromBeginning(false);
                    break;
                case "PlayFromPosition":
                    PlayFromPosition();
                    break;
                case "PlayNext":
                    PlayNext();
                    break;
                case "Replay":
                    PlayFromBeginning(true);
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        private void PlayFromBeginning(bool isPopupDisplayed)
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                IsPlaybackFromBeginning = true,
                WasPlaybackPopupShown = isPopupDisplayed,
                NextMediaElement = NextMediaElement
            });
        }

        private void PlayFromPosition()
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                IsPlaybackFromBeginning = false,
                WasPlaybackPopupShown = true,
                NextMediaElement = NextMediaElement
            });
        }

        private void PlayNext()
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = NextMediaElement,
                IsPlaybackFromBeginning = true,
                WasPlaybackPopupShown = true,
                NextMediaElement = NextAfterMediaElement
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
            Globals.Instance.UIDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                AutoPlayNextTimeLeft--;
                if (AutoPlayNextTimeLeft == 0)
                {
                    _autoPlaybackTimer.Stop();
                    PlayNext();
                }
            });
        }

        public void StopTimer()
        {
            _autoPlaybackTimer.Stop();
        }
    }
}