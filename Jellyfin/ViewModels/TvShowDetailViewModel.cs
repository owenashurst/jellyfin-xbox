using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;

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
                RaisePropertyChanged(nameof(SeasonEpisodesText));
                RaisePropertyChanged(nameof(SelectedSeason));

                if (value != null)
                {
                    SelectedSeasonChanged(value);
                }
            }
        }

        #endregion

        #region SeasonEpisodesText

        public string SeasonEpisodesText
        {
            get
            {
                if (SelectedSeason == null)
                {
                    return string.Empty;
                }

                if (SelectedSeasonEpisodes == null)
                {
                    return string.Empty;
                }

                if (!SelectedSeasonEpisodes.Any())
                {
                    return SelectedSeason.Name;
                }

                return SelectedSeason.Name + " • " + SelectedSeasonEpisodes.Count + " Episodes";
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

            foreach (TvShowSeason season in await _tvShowService.GetSeasonsBy((TvShow)tvShow))
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
                await _tvShowService.GetEpisodesBy((TvShow)SelectedMediaElement, season))
            {
                if (season.TvShowEpisodes.All(q => q.Id != episode.Id))
                {
                    season.TvShowEpisodes.Add(episode);
                }
            }

            SelectedSeasonEpisodes = season.TvShowEpisodes;
            RaisePropertyChanged(nameof(SelectedSeasonEpisodes));
            RaisePropertyChanged(nameof(SeasonEpisodesText));
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