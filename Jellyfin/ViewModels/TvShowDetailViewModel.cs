using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class TvShowDetailViewModel : JellyfinViewModelBase
    {
        #region Properties

        #region SelectedTvShow

        private TvShow _selectedTvShow;

        public TvShow SelectedTvShow
        {
            get { return _selectedTvShow; }
            set
            {
                _selectedTvShow = value;
                RaisePropertyChanged(nameof(SelectedTvShow));
                RaisePropertyChanged(nameof(FormattedResumeText));
            }
        }

        #endregion

        #region FormattedResumeText

        public string FormattedResumeText
        {
            get
            {
                if (SelectedTvShow == null)
                {
                    return string.Empty;
                }

                StringBuilder bld = new StringBuilder();
                bld.Append("Resume from ");

                if (SelectedTvShow.PlaybackPosition.Hours > 0)
                {
                    bld.Append(SelectedTvShow.PlaybackPosition.Hours).Append(":");
                    bld.Append(SelectedTvShow.PlaybackPosition.Minutes.ToString().PadLeft(2, '0')).Append(":");
                }
                else
                {
                    bld.Append(SelectedTvShow.PlaybackPosition.Minutes).Append(":");
                }

                
                bld.Append(SelectedTvShow.PlaybackPosition.Seconds.ToString().PadLeft(2, '0'));

                return bld.ToString();
            }
        }

        #endregion

        #region RelatedTvShows

        private ObservableCollection<TvShow> _relatedTvShows = new ObservableCollection<TvShow>();

        public ObservableCollection<TvShow> RelatedTvShows
        {
            get { return _relatedTvShows; }
            set
            {
                _relatedTvShows = value;
                RaisePropertyChanged(nameof(RelatedTvShows));
            }
        }

        #endregion

        /// <summary>
        /// Reference for the movie service.
        /// </summary>
        private readonly ITvShowService _tvShowService;

        /// <summary>
        /// Reference for the playback info service.
        /// </summary>
        private readonly IPlaybackInfoService _playbackInfoService;

        #endregion

        #region ctor

        public TvShowDetailViewModel(ITvShowService tvShowService, IPlaybackInfoService playbackInfoService)
        {
            _tvShowService = tvShowService ??
                            throw new ArgumentNullException(nameof(tvShowService));

            _playbackInfoService = playbackInfoService ??
                                   throw new ArgumentNullException(nameof(tvShowService));
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
                case "PlayFromBeginning":
                    PlayFromBeginning(false);
                    break;
                case "PlayFromPosition":
                    PlayFromPosition();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        public async Task GetTvShowDetails(MediaElementBase tvShow)
        {
            RelatedTvShows.Clear();
            SelectedTvShow = await _tvShowService.GetTvShowDetails(tvShow.Id);

            SelectedTvShow.Seasons = new ObservableCollection<TvShowSeason>(
                await _tvShowService.GetSeasonsBy(tvShow.Id));

            //foreach (TvShow relatedTvShow in await _tvShowService.GetRelatedTvShows(tvShow.Id))
            //{
            //    RelatedTvShows.Add(relatedTvShow);
            //}

            if (SelectedTvShow.PlaybackInformation == null)
            {
                SelectedTvShow.PlaybackInformation = await _playbackInfoService.GetPlaybackInformation(tvShow.Id);
            }
        }

        private void Play()
        {
            if (SelectedTvShow.PlaybackPosition != TimeSpan.Zero && SelectedTvShow.PlaybackPosition.TotalMinutes > 2 && SelectedTvShow.PlaybackRemaining.TotalMinutes > 2)
            {
                NavigationService.Navigate(typeof(PlaybackConfirmationView), SelectedTvShow);
            }
            else
            {
                PlayFromBeginning(false);
            }
        }

        private void PlayFromBeginning(bool isPopupDisplayed)
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedTvShow,
                IsPlaybackFromBeginning = true,
                WasPlaybackPopupShown = isPopupDisplayed
            });
        }

        private void PlayFromPosition()
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedTvShow,
                IsPlaybackFromBeginning = false,
                WasPlaybackPopupShown = true
            });
        }

        #endregion

        public bool HandleKeyPressed(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Escape:
                    NavigationService.GoBack();
                    return true;
                default:
                    return false;
            }
        }
    }
}