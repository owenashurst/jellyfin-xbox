using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
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

        #region DisplayedMovies

        private ObservableCollectionEx<Movie> _displayedMovies = new ObservableCollectionEx<Movie>();

        public ObservableCollectionEx<Movie> DisplayedMovies
        {
            get { return _displayedMovies; }
            set
            {
                _displayedMovies = value;
                RaisePropertyChanged(nameof(DisplayedMovies));
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

        public string ItemsCount
        {
            get { return $"{DisplayedMovies.Count} items"; }
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
        /// Reference for the movie service.
        /// </summary>
        private readonly IMovieService _movieService;

        #endregion

        #region ctor

        public MovieListViewModel(IMovieService movieService, IPersonalizeService personalizeService, ILogManager logManager)
            : base(personalizeService, logManager)
        {
            _movieService = movieService ??
                throw new ArgumentNullException(nameof(movieService));

            DisplayedMovies.CollectionChanged += MoviesOnCollectionChanged;
        }

        #endregion

        #region Additional methods

        private void MoviesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(ItemsCount));
        }

        [LogMethod]
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
                case "OrderByReleaseDate":
                    LastSortCommand = commandParameter;
                    OrderByReleaseDate();
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

        private void OrderByReleaseDate()
        {
            if (IsAscending)
            {
                OrderByDescending(movie => movie.PremiereDate);
            }
            else
            {
                OrderBy(movie => movie.PremiereDate);
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
        /// Orders the movie array by the passed predicate in descending.
        /// </summary>
        /// <param name="predicate"></param>
        private void OrderByDescending(Func<Movie, object> predicate)
        {
            DisplayedMovies.Clear();
            foreach (var movie in Movies.OrderByDescending(predicate))
            {
                DisplayedMovies.Add(movie);
            }

            RaisePropertyChanged(nameof(DisplayedMovies));
        }

        /// <summary>
        /// Orders the movie array by the passed predicate in ascending.
        /// </summary>
        /// <param name="predicate"></param>
        private void OrderBy(Func<Movie, object> predicate)
        {
            DisplayedMovies.Clear();
            foreach (var movie in Movies.OrderBy(predicate))
            {
                DisplayedMovies.Add(movie);
            }

            RaisePropertyChanged(nameof(DisplayedMovies));
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
                    _logManager.LogDebug("Movies already loaded, skipping to reload.");
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

                OrderByName();
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