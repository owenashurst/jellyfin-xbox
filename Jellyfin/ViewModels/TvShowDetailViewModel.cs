using System;
using System.Linq;
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
                RaisePropertyChanged(nameof(SeasonEpisodesText));
                RaisePropertyChanged(nameof(SelectedSeason));

                if (value != null)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    SelectedSeasonChanged(value);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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

        public override void Execute(string commandParameter)
        {
            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            switch (commandParameter)
            {
                case "Play":
                    Play();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }

            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task Play()
        {
            int currentSelectedNumber = ((TvShowEpisode)SelectedMediaElement).IndexNumber;
            TvShowEpisode[] remainingSeasonEpisodes =
                ((TvShowEpisode)SelectedMediaElement).Season.TvShowEpisodes
                .Where(q => q.IndexNumber > currentSelectedNumber)
                .OrderBy(q => q.IndexNumber).ToArray();

            NavigationService.Navigate(typeof(PlaybackConfirmationView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                Playlist = remainingSeasonEpisodes,
            });
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