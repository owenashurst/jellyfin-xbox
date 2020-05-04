using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;

namespace Jellyfin.ViewModels
{
    /// <summary>
    /// View model for listing the tv shows available.
    /// </summary>
    public class TvShowListViewModel : MediaElementListViewModelBase
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
                RaisePropertyChanged(nameof(ContinueWatchingTvShows));
            }
        }

        #endregion

        #region ContinueWatchingTvShows

        /// <summary>
        /// List of tv shows which has already been started.
        /// </summary>
        public ObservableCollectionEx<TvShow> ContinueWatchingTvShows
        {
            get
            {
                if (TvShows == null)
                {
                    return new ObservableCollectionEx<TvShow>();
                }

                List<TvShow> tvShows = TvShows.Where(q => q.PlaybackPosition.Ticks > 0).ToList();
                return new ObservableCollectionEx<TvShow>(tvShows);
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

        #endregion

        #region ctor

        public TvShowListViewModel(ITvShowService tvShowService)
        {
            _tvShowService = tvShowService ??
                            throw new ArgumentNullException(nameof(tvShowService));
        }

        #endregion

        #region Additional methods

        protected override void Execute(string commandParameter)
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
        public async Task Load()
        {
            if (!TvShows.Any())
            {
                IList<TvShow> tvShows = (await _tvShowService.GetTvShows()).ToList();
                foreach (TvShow tvShow in tvShows.OrderBy(q => q.Name))
                {
                    TvShows.Add(tvShow);
                }

                RaisePropertyChanged(nameof(ContinueWatchingTvShows));
            }
        }

        #endregion
    }
}