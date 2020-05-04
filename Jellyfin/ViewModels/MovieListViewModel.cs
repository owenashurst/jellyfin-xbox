using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;

namespace Jellyfin.ViewModels
{
    /// <summary>
    /// View model for listing the movies available.
    /// </summary>
    public class MovieListViewModel : MediaElementListViewModelBase
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
                RaisePropertyChanged(nameof(ContinueWatchingMovies));
                RaisePropertyChanged(nameof(RecentlyReleasedMovies));
                RaisePropertyChanged(nameof(MoviesFirstFavoriteGenre));
                RaisePropertyChanged(nameof(MoviesSecondFavoriteGenre));
                RaisePropertyChanged(nameof(FirstFavoriteGenre));
                RaisePropertyChanged(nameof(SecondFavoriteGenre));
            }
        }

        #endregion

        #region ContinueWatchingMovies

        /// <summary>
        /// List of movies which has already been started.
        /// </summary>
        public ObservableCollectionEx<Movie> ContinueWatchingMovies
        {
            get
            {
                if (Movies == null)
                {
                    return new ObservableCollectionEx<Movie>();
                }

                List<Movie> movies = Movies.Where(q => q.PlaybackPosition.Ticks > 0).ToList();
                return new ObservableCollectionEx<Movie>(movies);
            }
        }

        #endregion

        public ObservableCollectionEx<Movie> RecentlyReleasedMovies
        {
            get
            {
                if (Movies == null)
                {
                    return new ObservableCollectionEx<Movie>();
                }

                List<Movie> movies = Movies
                    .Where(q => q.PremiereDate > DateTime.Now.AddMonths(-18))
                    .OrderByDescending(q => q.PremiereDate)
                    .Take(20).ToList();
                return new ObservableCollectionEx<Movie>(movies);
            }
        }

        #region MovieGenres

        private List<string> _movieGenres = new List<string>();

        public List<string> MovieGenres
        {
            get
            {
                if (!_movieGenres.Any())
                {
                    if (Movies == null)
                    {
                        return new List<string>();
                    }

                    _movieGenres = _movies.SelectMany(q => q.Genres).GroupBy(q => q).Select(group => group.Key).OrderBy(x => x).ToList();
                }
                return _movieGenres;
            }
        }

        #endregion

        public string FirstFavoriteGenre
        {
            get
            {
                if (Movies == null)
                {
                    return string.Empty;
                }

                if (!MovieGenres.Any())
                {
                    return string.Empty;
                }

                return MovieGenres[0];
            }
        }

        public string SecondFavoriteGenre
        {
            get
            {
                if (Movies == null)
                {
                    return string.Empty;
                }

                if (MovieGenres.Count < 2)
                {
                    return string.Empty;
                }

                return MovieGenres[1];
            }
        }

        public ObservableCollectionEx<Movie> MoviesFirstFavoriteGenre
        {
            get
            {
                if (Movies == null)
                {
                    return new ObservableCollectionEx<Movie>();
                }
                
                string mostFavoriteGenre = FirstFavoriteGenre;
                if (string.IsNullOrEmpty(mostFavoriteGenre))
                {
                    return new ObservableCollectionEx<Movie>();
                }

                List<Movie> movies = Movies
                    .Where(q => q.Genres.Any(w => w == mostFavoriteGenre))
                    .OrderByDescending(q => q.OfficialRating)
                    .Take(20).ToList();
                return new ObservableCollectionEx<Movie>(movies);
            }
        }

        public ObservableCollectionEx<Movie> MoviesSecondFavoriteGenre
        {
            get
            {
                if (Movies == null)
                {
                    return new ObservableCollectionEx<Movie>();
                }
                
                string secondFavoriteGenre = SecondFavoriteGenre;
                if (string.IsNullOrEmpty(secondFavoriteGenre))
                {
                    return new ObservableCollectionEx<Movie>();
                }

                List<Movie> movies = Movies
                    .Where(q => q.Genres.Any(w => w == secondFavoriteGenre))
                    .OrderByDescending(q => q.OfficialRating)
                    .Take(20).ToList();
                return new ObservableCollectionEx<Movie>(movies);
            }
        }


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

        #endregion

        #region ctor

        public MovieListViewModel(IMovieService movieService)
        {
            _movieService = movieService ??
                throw new ArgumentNullException(nameof(movieService));
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
        public async Task Load()
        {
            if (!Movies.Any())
            {
                IList<Movie> movies = (await _movieService.GetMovies()).ToList();
                foreach (Movie movie in movies.OrderBy(q => q.Name))
                {
                    Movies.Add(movie);
                }

                RaisePropertyChanged(nameof(ContinueWatchingMovies));
                RaisePropertyChanged(nameof(RecentlyReleasedMovies));
                RaisePropertyChanged(nameof(MoviesFirstFavoriteGenre));
                RaisePropertyChanged(nameof(MoviesSecondFavoriteGenre));
                RaisePropertyChanged(nameof(FirstFavoriteGenre));
                RaisePropertyChanged(nameof(SecondFavoriteGenre));
            }
        }

        #endregion
    }
}