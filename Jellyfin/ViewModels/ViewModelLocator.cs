using Jellyfin.Logging;
using Jellyfin.Services.Interfaces;
using Unity;

namespace Jellyfin.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary> 
    public class ViewModelLocator
    {
        #region Properties

        /// <summary>
        /// Reference for Unity Container.
        /// </summary>
        private IUnityContainer _container;

        #endregion

        #region ctor

        public ViewModelLocator()
        {
            ConfigureViewModelMappings();
        }

        private void ConfigureViewModelMappings()
        {
            _container = Globals.Instance.Container;

            ILogManager logManager = _container.Resolve<ILogManager>();
            IMovieService movieService = _container.Resolve<IMovieService>();
            ITvShowService tvShowService = _container.Resolve<ITvShowService>();
            IPlaybackInfoService playbackInfoService = _container.Resolve<IPlaybackInfoService>();
            ILoginService loginService = _container.Resolve<ILoginService>();
            ISettingsService settingsService = _container.Resolve<ISettingsService>();
            IJellyfinNavigationService navigationService = _container.Resolve<IJellyfinNavigationService>();
            IReportProgressService reportProgressService = _container.Resolve<IReportProgressService>();
            IPersonalizeService personalizeService = _container.Resolve<IPersonalizeService>();

            _container.RegisterInstance(new MainViewModel(settingsService, navigationService));
            
            _container.RegisterInstance(new MovieListViewModel(movieService, personalizeService, logManager));
            _container.RegisterInstance(new MovieDetailViewModel(movieService, playbackInfoService, personalizeService, logManager));
            
            _container.RegisterInstance(new TvShowListViewModel(tvShowService, personalizeService, logManager));
            _container.RegisterInstance(new TvShowDetailViewModel(tvShowService, playbackInfoService, personalizeService, logManager));
            _container.RegisterInstance(new TvShowEpisodeDetailViewModel(tvShowService, playbackInfoService, personalizeService, logManager));

            _container.RegisterInstance(new MediaPlaybackViewModel(reportProgressService, logManager));
            _container.RegisterInstance(new PlaybackConfirmationViewModel(playbackInfoService, personalizeService, logManager));
            _container.RegisterInstance(new LoginViewModel(loginService, settingsService));
        }

        #endregion

        #region Additional methods

        /// <summary>
        /// Mapping for Main Page - Main View model.
        /// </summary>
        public MainViewModel MainPage
        {
            get => _container.Resolve<MainViewModel>();
        }

        /// <summary>
        /// Mapping for Movie Details Page - Movie Details View model.
        /// </summary>
        public MovieDetailViewModel MovieDetailPage
        {
            get => _container.Resolve<MovieDetailViewModel>();
        }

        /// <summary>
        /// Mapping for TV Show Details Page - TV Show Details View model.
        /// </summary>
        public TvShowDetailViewModel TvShowDetailPage
        {
            get => _container.Resolve<TvShowDetailViewModel>();
        }

        /// <summary>
        /// Mapping for Playback confirmation page - Movie Details View model.
        /// </summary>
        public PlaybackConfirmationViewModel PlaybackConfirmationPage
        {
            get => _container.Resolve<PlaybackConfirmationViewModel>();
        }

        /// <summary>
        /// Mapping for Movie List Page - Movie list View Model.
        /// </summary>
        public MovieListViewModel MovieListPage
        {
            get => _container.Resolve<MovieListViewModel>();
        }

        /// <summary>
        /// Mapping for TV Show List Page - TV Show list View Model.
        /// </summary>
        public TvShowListViewModel TvShowListPage
        {
            get => _container.Resolve<TvShowListViewModel>();
        }

        /// <summary>
        /// Mapping for Media Playback Page - Media Playback View Model.
        /// </summary>
        public MediaPlaybackViewModel MediaPlaybackPage
        {
            get => _container.Resolve<MediaPlaybackViewModel>();
        }

        /// <summary>
        /// Mapping for TV show Episode page - TV show Episode view model.
        /// </summary>
        public TvShowEpisodeDetailViewModel TvShowEpisodeDetailPage
        {
            get => _container.Resolve<TvShowEpisodeDetailViewModel>();
        }

        /// <summary>
        /// Mapping for Login Page - Login View Model.
        /// </summary>
        public LoginViewModel LoginPage
        {
            get => _container.Resolve<LoginViewModel>();
        }
        
        #endregion
    }
}
