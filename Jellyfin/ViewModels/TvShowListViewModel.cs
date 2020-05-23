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

        #region ContinueWatchingEpisodes

        private ObservableCollectionEx<TvShowEpisode> _continueWatchingEpisodes =
            new ObservableCollectionEx<TvShowEpisode>();

        public ObservableCollectionEx<TvShowEpisode> ContinueWatchingEpisodes
        {
            get { return _continueWatchingEpisodes; }
            set
            {
                _continueWatchingEpisodes = value;
                RaisePropertyChanged(nameof(ContinueWatchingEpisodes));
            }
        }

        #endregion

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

        /// <summary>
        /// Reference for the log manager.
        /// </summary>
        private readonly ILogManager _logManager;

        #endregion

        #region ctor

        public TvShowListViewModel(ITvShowService tvShowService, ILogManager logManager)
        {
            _tvShowService = tvShowService ??
                             throw new ArgumentNullException(nameof(tvShowService));

            _logManager = logManager ??
                          throw new ArgumentNullException(nameof(logManager));
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
                case "OrderBy":
                    // TODO smurancsik: add correct logging
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
                    if (ContinueWatchingEpisodes.All(q => q.Id != tvShowEpisode.Id))
                    {
                        ContinueWatchingEpisodes.Add(tvShowEpisode);

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

                                // It performs creating the graph, I am still in doubt if it's a good idea or not.
                                await _tvShowService.GetEpisodesBy(correspondingTvShow, correspondingSeason);
                            }
                        }
                    }
                }
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