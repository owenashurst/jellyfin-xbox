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
    /// View model for listing the movies available.
    /// </summary>
    public class MovieListViewModel : NavigableMediaElementViewModelBase
    {
        #region Properties

        #region Movies

        private ObservableCollectionEx<Movie> _movies = new ObservableCollectionEx<Movie>();

        public ObservableCollectionEx<Movie> Movies
        {
            get { return _movies; }
            set
            {
                _movies = value;
                RaisePropertyChanged(nameof(Movies));
            }
        }

        #endregion

        /// <summary>
        /// List of movies which has already been started.
        /// </summary>
        public ObservableCollectionEx<Movie> ContinueWatchingMovies { get; private set; }

        public ObservableCollectionEx<Movie> RecentlyReleasedMovies { get; private set; }

        public ObservableCollectionEx<Movie> MoviesFirstFavoriteGenre { get; private set; }

        public ObservableCollectionEx<Movie> MoviesSecondFavoriteGenre { get; private set; }

        public List<string> MovieGenres { get; private set; }

        public string FirstFavoriteGenre { get; private set; }

        public string SecondFavoriteGenre { get; private set; }

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
        /// Reference for the movie service.
        /// </summary>
        private readonly IMovieService _movieService;

        /// <summary>
        /// Reference for the log manager.
        /// </summary>
        private readonly ILogManager _logManager;

        #endregion

        #region ctor

        public MovieListViewModel(IMovieService movieService, ILogManager logManager)
        {
            _movieService = movieService ??
                throw new ArgumentNullException(nameof(movieService));

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
        /// Opens recommendations page: the movies recently
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
        /// Loads all the movies available.
        /// </summary>
        [LogMethod]
        public async Task Load()
        {
            IsLoading = true;

            try
            {
                if (Movies.Any())
                {
                    return;
                }

                IList<Movie> movies = (await _movieService.GetMovies()).ToList();
                foreach (Movie movie in movies.OrderBy(q => q.Name))
                {
                    Movies.Add(movie);
                }

                ContinueWatchingMovies = new ObservableCollectionEx<Movie>(
                    Movies.Where(q => q.PlaybackPosition.Ticks > 0));
                RaisePropertyChanged(nameof(ContinueWatchingMovies));

                RecentlyReleasedMovies = new ObservableCollectionEx<Movie>(Movies
                    .Where(q => q.PremiereDate > DateTime.Now.AddMonths(-18))
                    .OrderByDescending(q => q.PremiereDate)
                    .Take(20).ToList());

                RaisePropertyChanged(nameof(RecentlyReleasedMovies));

                MovieGenres = _movies.SelectMany(q => q.Genres).GroupBy(q => q).Select(group => group.Key)
                    .OrderBy(x => x).ToList();
                FirstFavoriteGenre = MovieGenres[0];
                SecondFavoriteGenre = MovieGenres[1];

                MoviesFirstFavoriteGenre = new ObservableCollectionEx<Movie>(Movies
                    .Where(q => q.Genres.Any(w => w == FirstFavoriteGenre))
                    .OrderByDescending(q => q.OfficialRating)
                    .Take(20).ToList());
                RaisePropertyChanged(nameof(MoviesFirstFavoriteGenre));

                MoviesSecondFavoriteGenre = new ObservableCollectionEx<Movie>(Movies
                    .Where(q => q.Genres.Any(w => w == SecondFavoriteGenre))
                    .OrderByDescending(q => q.OfficialRating)
                    .Take(20).ToList());
                RaisePropertyChanged(nameof(MoviesSecondFavoriteGenre));
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, "An error occurred while loading movies.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}