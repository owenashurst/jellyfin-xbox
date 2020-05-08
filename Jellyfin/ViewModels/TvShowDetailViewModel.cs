using System;
using System.Threading.Tasks;
using Windows.System;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class TvShowDetailViewModel : NavigableMediaElementViewModelBase
    {
        #region Properties

        #region RelatedTvShows

        private ObservableCollectionEx<TvShow> _relatedTvShows = new ObservableCollectionEx<TvShow>();

        public ObservableCollectionEx<TvShow> RelatedTvShows
        {
            get { return _relatedTvShows; }
            set
            {
                _relatedTvShows = value;
                RaisePropertyChanged(nameof(RelatedTvShows));
            }
        }

        #endregion

        #region SelectedSeason

        private TvShowSeason _selectedSeason;

        public TvShowSeason SelectedSeason
        {
            get { return _selectedSeason; }
            set
            {
                _selectedSeason = value;
                RaisePropertyChanged(nameof(SelectedSeason));

                if (value != null)
                {
                    SelectedSeasonChanged(value);
                }
            }
        }

        #endregion

        #region SelectedSeasonEpisodes

        private ObservableCollectionEx<TvShowEpisode> _selectedSeasonEpisodes = new ObservableCollectionEx<TvShowEpisode>();

        public ObservableCollectionEx<TvShowEpisode> SelectedSeasonEpisodes
        {
            get { return _selectedSeasonEpisodes; }
            set
            {
                _selectedSeasonEpisodes = value;
                RaisePropertyChanged(nameof(SelectedSeasonEpisodes));
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
            SelectedMediaElement = await _tvShowService.GetTvShowDetails(tvShow.Id);

            TvShow retrievedTvShow = (TvShow) SelectedMediaElement;

            foreach (TvShowSeason season in await _tvShowService.GetSeasonsBy(tvShow.Id))
            {
                retrievedTvShow.Seasons.Add(season);
            }

            SelectedSeason = retrievedTvShow.Seasons[0];

            //foreach (TvShow relatedTvShow in await _tvShowService.GetRelatedTvShows(tvShow.Id))
            //{
            //    RelatedTvShows.Add(relatedTvShow);
            //}

            if (retrievedTvShow.PlaybackInformation == null)
            {
                retrievedTvShow.PlaybackInformation = await _playbackInfoService.GetPlaybackInformation(tvShow.Id);
            }
        }

        private async Task SelectedSeasonChanged(TvShowSeason season)
        {
            foreach (TvShowEpisode episode in
                await _tvShowService.GetEpisodesBy(SelectedMediaElement.Id, season.Id))
            {
                season.TvShowEpisodes.Add(episode);
            }

            SelectedSeasonEpisodes = season.TvShowEpisodes;
            RaisePropertyChanged(nameof(SelectedSeasonEpisodes));
        }

        private void Play()
        {
            if (SelectedMediaElement.PlaybackPosition != TimeSpan.Zero && SelectedMediaElement.PlaybackPosition.TotalMinutes > 2 && SelectedMediaElement.PlaybackRemaining.TotalMinutes > 2)
            {
                NavigationService.Navigate(typeof(PlaybackConfirmationView), SelectedMediaElement);
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
                SelectedMediaElement = SelectedMediaElement,
                IsPlaybackFromBeginning = true,
                WasPlaybackPopupShown = isPopupDisplayed
            });
        }

        private void PlayFromPosition()
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
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