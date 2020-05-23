using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Jellyfin.Core;
using Jellyfin.Extensions;
using Jellyfin.Logging;
using Jellyfin.Models;
using Jellyfin.Models.ServiceModels;
using Jellyfin.Models.ServiceModels.Movie;
using Jellyfin.Services.Interfaces;
using Newtonsoft.Json;

namespace Jellyfin.Services
{
    public class MovieService : MediaQueryServiceBase, IMovieService
    {
        #region Properties

        public string ListMoviesEndpoint
        {
            get =>
                $"{Globals.Instance.Host}/Users/{Globals.Instance.User.Id}/Items?IncludeItemTypes=Movie&Recursive=true&Fields=PrimaryImageAspectRatio%2CMediaSourceCount%2CBasicSyncInfo%2CGenres&ImageTypeLimit=1&EnableImageTypes=Primary%2CBackdrop%2CBanner%2CThumb&StartIndex=0&Limit=100000"
            ;
        }

        public string GetMovieDetailsEndpoint
        {
            get => $"{Globals.Instance.Host}/Users/{Globals.Instance.User.Id}/Items/";
        }

        public string GetRelatedMoviesEndpoint
        {
            get => $"{Globals.Instance.Host}/Items/{{0}}/Similar?userId={Globals.Instance.User.Id}&limit=12&fields=PrimaryImageAspectRatio";
        }

        /// <summary>
        /// Reference for the movie adapter.
        /// </summary>
        private readonly IAdapter<MovieItem, Movie> _movieAdapter;

        /// <summary>
        /// Reference for the movie details adapter.
        /// </summary>
        private readonly IAdapter<MovieDetailsResult, Movie> _movieDetailsAdapter;

        /// <summary>
        /// Reference for the log manager.
        /// </summary>
        private readonly ILogManager _logManager;

        #endregion

        #region ctor

        public MovieService(
            IAdapter<MovieItem, Movie> movieAdapter,
            IAdapter<MovieDetailsResult, Movie> movieDetailsAdapter,
            IImageService imageService,
            ILogManager logManager) : base(imageService)
        {
            _movieAdapter = movieAdapter ??
                throw new ArgumentNullException(nameof(movieAdapter));

            _movieDetailsAdapter = movieDetailsAdapter ??
                throw new ArgumentNullException(nameof(movieDetailsAdapter));

            _logManager = logManager ??
                throw new ArgumentNullException(nameof(logManager));
        }
        
        #endregion

        #region Additional methods

        [LogMethod]
        public async Task<IEnumerable<Movie>> GetMovies()
        {
            List<Movie> movieList = new List<Movie>();

            try
            {

                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    HttpResponseMessage result = await cli.GetAsync(ListMoviesEndpoint);

                    if (!result.IsSuccessStatusCode)
                    {
                        return new List<Movie>();
                    }

                    string jsonResult = await result.Content.ReadAsStringAsync();

                    JellyfinMovieResult resultSet = JsonConvert.DeserializeObject<JellyfinMovieResult>(jsonResult);

                    foreach (MovieItem item in resultSet.Items)
                    {
                        Movie movie = _movieAdapter.Convert(item);
                        movieList.Add(movie);
                        ImageDownloadQueue.EnqueueTask(movie);
                    }
                }
            } catch (Exception xc)
            {
                _logManager.LogError(xc, "An error occurred while getting movies.");
            }

            return movieList;
        }

        [LogMethod]
        public async Task<Movie> GetMovieDetails(string movieId)
        {
            try
            {
                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    HttpResponseMessage result = cli.GetAsync($"{GetMovieDetailsEndpoint}{movieId}").Result;

                    if (!result.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    string jsonResult = await result.Content.ReadAsStringAsync();

                    MovieDetailsResult resultSet = JsonConvert.DeserializeObject<MovieDetailsResult>(jsonResult);

                    var item = _movieDetailsAdapter.Convert(resultSet);
                    ImageDownloadQueue.EnqueueTask(item);
                    return item;
                }
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, $"An error occurred while getting movie details for ID {movieId}.");
            }

            return null;
        }

        [LogMethod]
        public async Task<IEnumerable<Movie>> GetRelatedMovies(string movieId)
        {
            List<Movie> movieList = new List<Movie>();

            try
            {
                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    HttpResponseMessage result = await cli.GetAsync(string.Format(GetRelatedMoviesEndpoint, movieId));

                    if (!result.IsSuccessStatusCode)
                    {
                        return new List<Movie>();
                    }

                    string jsonResult = await result.Content.ReadAsStringAsync();

                    JellyfinMovieResult resultSet = JsonConvert.DeserializeObject<JellyfinMovieResult>(jsonResult);

                    foreach (MovieItem item in resultSet.Items)
                    {
                        Movie movie = _movieAdapter.Convert(item);
                        movieList.Add(movie);
                        ImageDownloadQueue.EnqueueTask(movie);
                    }
                }
            } catch (Exception xc)
            {
                _logManager.LogError(xc, $"An error occurred while getting related movies for ID {movieId}.");
            }

            return movieList;
        }

        #endregion
    }
}
