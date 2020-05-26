using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Jellyfin.Extensions;
using Jellyfin.Logging;
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

                return $"{SelectedSeason.Name} • {SelectedSeasonEpisodes.Count} Episodes";
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

        #region FirstAvailableEpisode

        private MediaElementBase _firstAvailableEpisode;

        public MediaElementBase FirstAvailableEpisode
        {
            get { return _firstAvailableEpisode; }
            set
            {
                if (_firstAvailableEpisode != value)
                {
                    _firstAvailableEpisode = value;
                    RaisePropertyChanged(nameof(FirstAvailableEpisode));
                }
            }
        }

        #endregion

        #region ContinueWatchingEpisode

        private MediaElementBase _continueWatchingEpisode;

        public MediaElementBase ContinueWatchingEpisode
        {
            get { return _continueWatchingEpisode; }
            set
            {
                if (_continueWatchingEpisode != value)
                {
                    _continueWatchingEpisode = value;
                    RaisePropertyChanged(nameof(ContinueWatchingEpisode));
                }
            }
        }

        #endregion

        #region NextUnplayedEpisode

        private MediaElementBase _nextUnplayedEpisode;

        public MediaElementBase NextUnplayedEpisode
        {
            get { return _nextUnplayedEpisode; }
            set
            {
                if (_nextUnplayedEpisode != value)
                {
                    _nextUnplayedEpisode = value;
                    RaisePropertyChanged(nameof(NextUnplayedEpisode));
                }
            }
        }

        #endregion

        #region FirstEpisodeFromCurrentSeason

        private MediaElementBase _firstEpisodeFromCurrentSeason;

        public MediaElementBase FirstEpisodeFromCurrentSeason
        {
            get { return _firstEpisodeFromCurrentSeason; }
            set
            {
                if (_firstEpisodeFromCurrentSeason != value)
                {
                    _firstEpisodeFromCurrentSeason = value;
                    RaisePropertyChanged(nameof(FirstEpisodeFromCurrentSeason));
                }
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

        public TvShowDetailViewModel(ITvShowService tvShowService, IPlaybackInfoService playbackInfoService, IPersonalizeService personalizeService, ILogManager logManager) : base(personalizeService, logManager)
        {
            _tvShowService = tvShowService ??
                            throw new ArgumentNullException(nameof(tvShowService));

            _playbackInfoService = playbackInfoService ??
                                   throw new ArgumentNullException(nameof(tvShowService));
        }

        #endregion

        #region ToolCommand Implementation

        /// <summary>
        /// Executes Tool Command requests coming from the UI.
        /// </summary>
        /// <param name="commandParameter"></param>
        [LogMethod]
        public override void Execute(string commandParameter)
        {
            #pragma warning disable CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
            switch (commandParameter)
            {
                case "PlayContinueWatchingEpisode":
                    PlayContinueWatchingEpisode();
                    break;
                case "PlayNextUnplayedEpisode":
                    PlayNextUnplayedEpisode();
                    break;
                case "PlayFirstEpisodeFromCurrentSeason":
                    PlayFirstEpisodeFromCurrentSeason();
                    Play();
                    break;
                case "PlayFirstAvailableEpisode":
                    PlayFirstAvailableEpisode();
                    break;
                case "Shuffle":
                    Shuffle();
                    break;
                case "Play":
                    Play();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }

            #pragma warning restore CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        #endregion

        #region Additional methods

        [LogMethod]
        public TvShowEpisode[] GetAllEpisodesAfter(MediaElementBase episode)
        {
            if (!(episode is TvShowEpisode))
            {
                _logManager.LogWarn("Get All Episodes After was called, but not on an episode. Grab a programmer!");
                return new TvShowEpisode[0];
            }

            var e = episode as TvShowEpisode;
            if (e.TvShow == null)
            {
                _logManager.LogWarn("Get All Episodes After was called, but the referenced episode's tv show was null. Grab a programmer!");
                return new TvShowEpisode[0];
            }

            var orderedEpisodeList = e.TvShow.AllEpisodes
                .OrderBy(q => q.SeasonNumber).ThenBy(q => q.IndexNumber).ToList();

            var toSlice = orderedEpisodeList.IndexOf(e);
            return orderedEpisodeList.Skip(toSlice).ToArray();
        }

        private void PlayContinueWatchingEpisode()
        {
            if (ContinueWatchingEpisode == null)
            {
                _logManager.LogWarn(
                    "PlayContinueWatchingEpisode was called, but the passed episode was null. Grab a programmer (and hide that button!)");
                return;
            }

            PlayImpl(ContinueWatchingEpisode);
        }

        private void PlayNextUnplayedEpisode()
        {
            if (NextUnplayedEpisode == null)
            {
                _logManager.LogWarn(
                    "PlayNextUnplayedEpisode was called, but the passed episode was null. Grab a programmer (and hide that button!)");
                return;
            }

            PlayImpl(NextUnplayedEpisode);
        }

        private void PlayFirstEpisodeFromCurrentSeason()
        {
            if (FirstEpisodeFromCurrentSeason == null)
            {
                _logManager.LogWarn(
                    "PlayFirstEpisodeFromCurrentSeason was called, but the passed episode was null. Grab a programmer (and hide that button!)");
                return;
            }

            PlayImpl(FirstEpisodeFromCurrentSeason);
        }

        private void PlayFirstAvailableEpisode()
        {
            if (FirstAvailableEpisode == null)
            {
                _logManager.LogWarn(
                    "PlayFirstAvailableEpisode was called, but the passed episode was null. Grab a programmer (and hide that button!)");
                return;
            }

            PlayImpl(FirstAvailableEpisode);
        }

        /// <summary>
        /// Then a user chooses an episode from the Season XX - YY Episodes block, it'll be executed here.
        /// </summary>
        /// <returns></returns>
        private void Play()
        {
            if (DirectPlayMediaElement == null)
            {
                _logManager.LogWarn(
                    "Play was called, but the passed episode was null. Grab a programmer (and hide that button!)");
                return;
            }

            PlayImpl(DirectPlayMediaElement);
        }

        /// <summary>
        /// Generates the playlist, and redirects to playback confirmation view.
        /// </summary>
        /// <param name="e"></param>
        private void PlayImpl(MediaElementBase e)
        {
            NavigationService.Navigate(typeof(PlaybackConfirmationView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = e,
                Playlist = GetAllEpisodesAfter(e)
            });
        }

        /// <summary>
        /// Starts playing in shuffle mode.
        /// </summary>
        private void Shuffle()
        {
            TvShow retrievedTvShow = (TvShow) SelectedMediaElement;
            TvShowEpisode[] randomOrderedEpisodes = retrievedTvShow.AllEpisodes.OrderBy(q => Guid.NewGuid()).ToArray();

            NavigationService.Navigate(typeof(PlaybackConfirmationView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = randomOrderedEpisodes[0],
                Playlist = randomOrderedEpisodes.Skip(1).ToArray()
            });
        }

        private void Reinitialize()
        {
            RelatedTvShows.Clear();

            FirstAvailableEpisode = null;
            ContinueWatchingEpisode = null;
            NextUnplayedEpisode = null;
            FirstEpisodeFromCurrentSeason = null;
        }

        [LogMethod]
        public async Task Load(MediaElementBase tvShow)
        {
            IsLoading = true;

            try
            {
                Reinitialize();

                await LoadTvShowDetails(tvShow);

                TvShow retrievedTvShow = await ConfiguredRetrievedTvShowDetails(tvShow);
                ConfigureSpecialPlaybackEpisodes(retrievedTvShow);

                await LoadRelatedTvShows(tvShow);
                await LoadPlaybackInformation(tvShow, retrievedTvShow);
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, $"An error occurred while initializing tv show detail for {SelectedMediaElement}.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ConfigureSpecialPlaybackEpisodes(TvShow retrievedTvShow)
        {
            // Retrieve S01E01; first episode from current season and currently playing episode.
            var firstAvailableSeasonWithEpisodes = retrievedTvShow.Seasons
                .OrderBy(q => q.IndexNumber)
                .FirstOrDefault(q => q.TvShowEpisodes.Any());

            if (firstAvailableSeasonWithEpisodes != null)
            {
                var firstAvailableEpisode = firstAvailableSeasonWithEpisodes.TvShowEpisodes
                    .OrderBy(q => q.IndexNumber)
                    .FirstOrDefault();

                if (FirstEpisodeFromCurrentSeason != firstAvailableEpisode)
                {
                    FirstAvailableEpisode = firstAvailableEpisode;
                    _logManager.LogDebug(
                        $"TV Show {retrievedTvShow}: first available episode: {FirstAvailableEpisode}");
                }
            }
            else
            {
                _logManager.LogWarn($"TV Show {retrievedTvShow}: no episode available. Grab a programmer!");
            }

            var orderedEpisodes = retrievedTvShow.AllEpisodes
                .OrderBy(q => q.SeasonNumber)
                .ThenBy(q => q.IndexNumber).ToList();

            var firstPartiallyWatchedEpisode =
                orderedEpisodes.FirstOrDefault(q => q.UserData.PlaybackPositionTicks > 0);

            if (firstPartiallyWatchedEpisode != null && firstPartiallyWatchedEpisode != FirstAvailableEpisode)
            {
                ContinueWatchingEpisode = firstPartiallyWatchedEpisode;
                var thisIndex = orderedEpisodes.IndexOf(firstPartiallyWatchedEpisode);
                _logManager.LogDebug(
                    $"TV Show {retrievedTvShow}: first partially watched episode: {ContinueWatchingEpisode}");

                if (orderedEpisodes.Count > thisIndex + 1)
                {
                    NextUnplayedEpisode = orderedEpisodes[thisIndex + 1];
                    _logManager.LogDebug($"TV Show {retrievedTvShow}: next unplayed episode: {NextUnplayedEpisode}");
                }
            }
            else
            {
                _logManager.LogDebug($"TV Show {retrievedTvShow}: no partially watched episode.");
            }
        }

        private async Task<TvShow> ConfiguredRetrievedTvShowDetails(MediaElementBase tvShow)
        {
            TvShow retrievedTvShow = (TvShow) SelectedMediaElement;

            foreach (TvShowSeason season in await _tvShowService.GetSeasonsBy((TvShow) tvShow))
            {
                retrievedTvShow.Seasons.Add(season);

                foreach (TvShowEpisode episode in
                    await _tvShowService.GetEpisodesBy((TvShow) SelectedMediaElement, season))
                {
                    if (season.TvShowEpisodes.Any(q => q.Id == episode.Id))
                    {
                        continue;
                    }

                    season.TvShowEpisodes.Add(episode);
                    retrievedTvShow.AllEpisodes.Add(episode);
                    episode.Season = season;
                    episode.TvShow = retrievedTvShow;
                }
            }

            SelectedSeason = retrievedTvShow.Seasons[0];
            return retrievedTvShow;
        }

        private async Task LoadTvShowDetails(MediaElementBase tvShow)
        {
            SelectedMediaElement = await _tvShowService.GetTvShowDetails(tvShow.Id);
        }

        private async Task LoadPlaybackInformation(MediaElementBase tvShow, TvShow retrievedTvShow)
        {
            if (retrievedTvShow.PlaybackInformation == null)
            {
                retrievedTvShow.PlaybackInformation = await _playbackInfoService.GetPlaybackInformation(tvShow.Id);
            }
        }

        /// <summary>
        /// Loads and displays related tv shows.
        /// </summary>
        /// <param name="tvShow"></param>
        /// <returns></returns>
        [LogMethod]
        private async Task LoadRelatedTvShows(MediaElementBase tvShow)
        {
            try
            {
                foreach (TvShow relatedTvShow in await _tvShowService.GetRelatedTvShows(tvShow.Id))
                {
                    RelatedTvShows.Add(relatedTvShow);
                }
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, "An error occurred while loading related TV shows.");
            }
        }

        [LogMethod]
        private void SelectedSeasonChanged(TvShowSeason season)
        {
            if (season.TvShowEpisodes.Any())
            {
                var firstEpisodeFromCurrentSeason = season.TvShowEpisodes.OrderBy(q => q.IndexNumber).FirstOrDefault();
                if (firstEpisodeFromCurrentSeason != null &&
                    firstEpisodeFromCurrentSeason != FirstAvailableEpisode &&
                    firstEpisodeFromCurrentSeason != ContinueWatchingEpisode &&
                    firstEpisodeFromCurrentSeason != NextUnplayedEpisode)
                {
                    FirstEpisodeFromCurrentSeason = firstEpisodeFromCurrentSeason;
                    _logManager.LogDebug($"TV Show {season.TvShow}: season {season} premiere: {FirstEpisodeFromCurrentSeason}");
                }
            }

            SelectedSeasonEpisodes = season.TvShowEpisodes;
            RaisePropertyChanged(nameof(SelectedSeasonEpisodes));
            RaisePropertyChanged(nameof(SeasonEpisodesText));
        }

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

        #endregion
    }
}