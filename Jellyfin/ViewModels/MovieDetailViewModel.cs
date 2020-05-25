using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Jellyfin.Logging;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class MovieDetailViewModel : NavigableMediaElementViewModelBase
    {
        #region Properties

        #region RelatedMovies

        private ObservableCollection<Movie> _relatedMovies = new ObservableCollection<Movie>();

        public ObservableCollection<Movie> RelatedMovies
        {
            get { return _relatedMovies; }
            set
            {
                _relatedMovies = value;
                RaisePropertyChanged(nameof(RelatedMovies));
            }
        }

        #endregion

        /// <summary>
        /// Reference for the movie service.
        /// </summary>
        private readonly IMovieService _movieService;

        /// <summary>
        /// Reference for the playback info service.
        /// </summary>
        private readonly IPlaybackInfoService _playbackInfoService;

        #endregion

        #region ctor

        public MovieDetailViewModel(
            IMovieService movieService, IPlaybackInfoService playbackInfoService,
            IPersonalizeService personalizeService, ILogManager logManager) : base(personalizeService, logManager)
        {
            _movieService = movieService ??
                    throw new ArgumentNullException(nameof(movieService));

            _playbackInfoService = playbackInfoService ??
                    throw new ArgumentNullException(nameof(playbackInfoService));
        }

        #endregion

        #region Additional methods

        [LogMethod]
        public override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Play":
                    Play();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        public async Task GetMovieDetails(Movie movie)
        {
            RelatedMovies.Clear();
            SelectedMediaElement = await _movieService.GetMovieDetails(movie.Id);

            foreach (Movie relatedMovie in await _movieService.GetRelatedMovies(movie.Id))
            {
                RelatedMovies.Add(relatedMovie);
            }

            if (SelectedMediaElement.PlaybackInformation == null)
            {
                SelectedMediaElement.PlaybackInformation = await _playbackInfoService.GetPlaybackInformation(movie.Id);
            }
        }

        /// <summary>
        /// Passes to playback confirmation view. This would determine whether the movie was already played for several minutes or not,
        /// and if so, it would display if the user would like to continue watching where s/he stopped watching from the last time,
        /// or start over.
        /// </summary>
        [LogMethod]
        private void Play()
        {
            NavigationService.Navigate(typeof(PlaybackConfirmationView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                Playlist = RelatedMovies.ToArray()
            });
        }

        [LogMethod]
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