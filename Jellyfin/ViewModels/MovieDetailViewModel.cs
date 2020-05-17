using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class MovieDetailViewModel : JellyfinViewModelBase
    {
        #region Properties

        #region SelectedMovie

        private Movie _selectedMovie;

        public Movie SelectedMovie
        {
            get { return _selectedMovie; }
            set
            {
                _selectedMovie = value;
                RaisePropertyChanged(nameof(SelectedMovie));
                RaisePropertyChanged(nameof(FormattedResumeText));
            }
        }

        #endregion

        #region FormattedResumeText

        public string FormattedResumeText
        {
            get
            {
                if (SelectedMovie == null)
                {
                    return string.Empty;
                }

                StringBuilder bld = new StringBuilder();
                bld.Append("Resume from ");

                if (SelectedMovie.PlaybackPosition.Hours > 0)
                {
                    bld.Append(SelectedMovie.PlaybackPosition.Hours).Append(":");
                    bld.Append(SelectedMovie.PlaybackPosition.Minutes.ToString().PadLeft(2, '0')).Append(":");
                }
                else
                {
                    bld.Append(SelectedMovie.PlaybackPosition.Minutes).Append(":");
                }

                
                bld.Append(SelectedMovie.PlaybackPosition.Seconds.ToString().PadLeft(2, '0'));

                return bld.ToString();
            }
        }

        #endregion

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

        public MovieDetailViewModel(IMovieService movieService, IPlaybackInfoService playbackInfoService)
        {
            _movieService = movieService ??
                    throw new ArgumentNullException(nameof(movieService));

            _playbackInfoService = playbackInfoService ??
                            throw new ArgumentNullException(nameof(playbackInfoService));
        }

        #endregion

        #region Additional methods

        public override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Play":
                    Play();
                    break;
                //case "PlayFromBeginning":
                //    PlayFromBeginning(false);
                //    break;
                //case "PlayFromPosition":
                //    PlayFromPosition();
                //    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        public async Task GetMovieDetails(Movie movie)
        {
            RelatedMovies.Clear();
            SelectedMovie = await _movieService.GetMovieDetails(movie.Id);

            foreach (Movie relatedMovie in await _movieService.GetRelatedMovies(movie.Id))
            {
                RelatedMovies.Add(relatedMovie);
            }

            if (SelectedMovie.PlaybackInformation == null)
            {
                SelectedMovie.PlaybackInformation = await _playbackInfoService.GetPlaybackInformation(movie.Id);
            }
        }

        private void Play()
        {
            NavigationService.Navigate(typeof(PlaybackConfirmationView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMovie,
                Playlist = RelatedMovies.ToArray()
            });
        }

        //private void PlayFromBeginning(bool isPopupDisplayed)
        //{
        //    NavigationService.Navigate(typeof(MediaPlaybackView), );
        //}

        //private void PlayFromPosition()
        //{
        //    NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
        //    {
        //        SelectedMediaElement = SelectedMovie,
        //        IsPlaybackFromBeginning = false,
        //        Playlist = RelatedMovies.ToArray()
        //    });
        //}

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