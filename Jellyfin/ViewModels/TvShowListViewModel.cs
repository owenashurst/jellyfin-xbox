using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Extensions;
using Jellyfin.Logging;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;

namespace Jellyfin.ViewModels
{
    /// <summary>
    /// View model for listing the tv shows available.
    /// </summary>
    public class TvShowListViewModel : NavigableMediaElementViewModelBase
    {
        #region Properties

        #region TvShows

        private ObservableCollectionEx<TvShow> _tvShows = new ObservableCollectionEx<TvShow>();

        public ObservableCollectionEx<TvShow> TvShows
        {
            get { return _tvShows; }
            set
            {
                _tvShows = value;
                RaisePropertyChanged(nameof(TvShows));
            }
        }

        #endregion

        #region DisplayedTvShows

        private ObservableCollectionEx<TvShow> _displayedTvShows = new ObservableCollectionEx<TvShow>();

        public ObservableCollectionEx<TvShow> DisplayedTvShows
        {
            get { return _displayedTvShows; }
            set
            {
                _displayedTvShows = value;
                RaisePropertyChanged(nameof(DisplayedTvShows));
            }
        }

        #endregion

        public ObservableCollectionEx<TvShowEpisode> ContinueWatchingTvShows { get; private set; }

        public ObservableCollectionEx<TvShow> RecentlyReleasedTvShows { get; private set; }

        public ObservableCollectionEx<TvShow> TvShowsFirstFavoriteGenre { get; private set; }

        public ObservableCollectionEx<TvShow> TvShowsSecondFavoriteGenre { get; private set; }

        public List<string> TvShowGenres { get; private set; }

        public string FirstFavoriteGenre { get; private set; }

        public string SecondFavoriteGenre { get; private set; }

        public string ItemsCount
        {
            get { return $"{DisplayedTvShows.Count} items"; }
        }

        /// <summary>
        /// Indicates whether the sort was ascending or descending the last time.
        /// </summary>
        public bool IsAscending { get; set; } = true;

        /// <summary>
        /// Contains last sort command.
        /// </summary>
        public string LastSortCommand { get; set; } = "OrderByName";

        #region IsRecommendationsOpened

        private bool _isRecommendationsOpened;

        /// <summary>
        /// Indicates whether the recommendations or the library view is opened.
        /// </summary>
        public bool IsRecommendationsOpened
        {
            get { return _isRecommendationsOpened; }
            set
            {
                _isRecommendationsOpened = value;
                RaisePropertyChanged(nameof(IsRecommendationsOpened));
            }
        }

        #endregion

        /// <summary>
        /// Reference for the tv show service.
        /// </summary>
        private readonly ITvShowService _tvShowService;

        #endregion

        #region ctor

        public TvShowListViewModel(ITvShowService tvShowService, IPersonalizeService personalizeService, ILogManager logManager)
            : base(personalizeService, logManager)
        {
            _tvShowService = tvShowService ??
                             throw new ArgumentNullException(nameof(tvShowService));

            ContinueWatchingTvShows = new ObservableCollectionEx<TvShowEpisode>();
        }

        #endregion

        #region Additional methods

        public override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "OpenRecommendations":
                    OpenRecommendations();
                    break;
                case "OpenLibrary":
                    OpenLibrary();
                    break;
                case "OrderByName":
                    LastSortCommand = commandParameter;
                    OrderByName();
                    break;
                case "OrderByRating":
                    LastSortCommand = commandParameter;
                    OrderByRating();
                    break;
                case "OrderByDateAdded":
                    LastSortCommand = commandParameter;
                    OrderByDateAdded();
                    break;
                case "OrderByRuntime":
                    LastSortCommand = commandParameter;
                    OrderByRuntime();
                    break;
                case "Ascending":
                    Ascending();
                    break;
                case "Descending":
                    Descending();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        /// <summary>
        /// Opens recommendations page: the tv shows recently
        /// started watching, then recommendations.
        /// </summary>
        private void OpenRecommendations()
        {
            IsRecommendationsOpened = true;
        }

        /// <summary>
        /// Opens the library, normally in alphabetic mode.
        /// </summary>
        private void OpenLibrary()
        {
            IsRecommendationsOpened = false;
        }

        private void Ascending()
        {
            if (IsAscending)
            {
                _logManager.LogDebug("Ascending command arrived, but the list is already sorted to ascending. Returning...");
                return;
            }

            IsAscending = true;
            Execute(LastSortCommand);
        }

        private void Descending()
        {
            if (!IsAscending)
            {
                _logManager.LogDebug("Descending command arrived, but the list is already sorted to descending. Returning...");
                return;
            }

            IsAscending = false;
            Execute(LastSortCommand);
        }

        /// <summary>
        /// Orders movies by name.
        /// </summary>
        private void OrderByName()
        {
            if (IsAscending)
            {
                OrderBy(movie => movie.Name);
            }
            else
            {
                OrderByDescending(movie => movie.Name);
            }
        }

        private void OrderByDateAdded()
        {
            if (IsAscending)
            {
                OrderBy(movie => movie.DateCreated);
            }
            else
            {
                OrderByDescending(movie => movie.DateCreated);
            }
        }

        private void OrderByRuntime()
        {
            if (IsAscending)
            {
                OrderBy(movie => movie.Runtime);
            }
            else
            {
                OrderByDescending(movie => movie.Runtime);
            }
        }

        /// <summary>
        /// Orders movies by name.
        /// </summary>
        private void OrderByRating()
        {
            if (IsAscending)
            {
                OrderByDescending(movie => movie.CommunityRating);
            }
            else
            {
                OrderBy(movie => movie.CommunityRating);
            }
        }

        /// <summary>
        /// Orders the tv show array by the passed predicate in descending.
        /// </summary>
        /// <param name="predicate"></param>
        private void OrderByDescending(Func<TvShow, object> predicate)
        {
            DisplayedTvShows.Clear();
            foreach (var movie in TvShows.OrderByDescending(predicate))
            {
                DisplayedTvShows.Add(movie);
            }

            RaisePropertyChanged(nameof(DisplayedTvShows));
        }

        /// <summary>
        /// Orders the tv show array by the passed predicate in ascending.
        /// </summary>
        /// <param name="predicate"></param>
        private void OrderBy(Func<TvShow, object> predicate)
        {
            DisplayedTvShows.Clear();
            foreach (var movie in TvShows.OrderBy(predicate))
            {
                DisplayedTvShows.Add(movie);
            }

            RaisePropertyChanged(nameof(DisplayedTvShows));
        }

        /// <summary>
        /// Loads all the tv shows available.
        /// </summary>
        [LogMethod]
        public async Task Load()
        {
            IsLoading = true;

            try
            {
                if (!TvShows.Any())
                {
                    IList<TvShow> tvShows = (await _tvShowService.GetTvShows()).ToList();
                    foreach (TvShow tvShow in tvShows.OrderBy(q => q.Name))
                    {
                        TvShows.Add(tvShow);
                    }
                }

                IList<TvShowEpisode> tvShowEpisodes = (await _tvShowService.GetContinueWatchingEpisodes()).ToList();
                foreach (TvShowEpisode tvShowEpisode in tvShowEpisodes)
                {
                    if (ContinueWatchingTvShows.All(q => q.Id != tvShowEpisode.Id))
                    {
                        ContinueWatchingTvShows.Add(tvShowEpisode);

                        TvShow correspondingTvShow = TvShows.FirstOrDefault(q => q.Id == tvShowEpisode.SeriesId);
                        if (correspondingTvShow != null)
                        {
                            tvShowEpisode.TvShow = correspondingTvShow;

                            TvShowSeason correspondingSeason =
                                correspondingTvShow.Seasons.FirstOrDefault(q => q.Id == tvShowEpisode.SeasonId);

                            if (correspondingSeason != null)
                            {
                                tvShowEpisode.Season = correspondingSeason;
                            }
                            else
                            {
                                IEnumerable<TvShowSeason> seasons =
                                    await _tvShowService.GetSeasonsBy(correspondingTvShow);

                                correspondingSeason =
                                    seasons.FirstOrDefault(q => q.Id == tvShowEpisode.SeasonId);

                                tvShowEpisode.Season = correspondingSeason;

                                await _tvShowService.GetEpisodesBy(correspondingTvShow, correspondingSeason);
                            }
                        }
                    }
                }

                OrderByName();
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, "An error occurred while loading tv shows.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}