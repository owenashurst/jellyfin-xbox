using System;
using Jellyfin.Core;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class MainViewModel : JellyfinViewModelBase
    {
        #region Properties
        
        /// <summary>
        /// Reference for the settings service.
        /// </summary>
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Reference for the navigation service.
        /// </summary>
        private readonly IJellyfinNavigationService _navigationService;

        #region IsShowMovies

        private bool _isShowMovies;

        public bool IsShowMovies
        {
            get { return _isShowMovies; }
            set
            {
                _isShowMovies = value;
                RaisePropertyChanged(nameof(IsShowMovies));
            }
        }

        #endregion

        #region IsShowTvShows

        private bool _isShowTvShows;

        public bool IsShowTvShows
        {
            get { return _isShowTvShows; }
            set
            {
                _isShowTvShows = value;
                RaisePropertyChanged(nameof(IsShowTvShows));
            }
        }

        #endregion

        #region IsShowMusic

        private bool _isShowMusic;

        public bool IsShowMusic
        {
            get { return _isShowMusic; }
            set
            {
                _isShowMusic = value;
                RaisePropertyChanged(nameof(IsShowMusic));
            }
        }

        #endregion
        
        #endregion

        #region ctor

        public MainViewModel(ISettingsService settingsService, IJellyfinNavigationService jellyfinNavigationService)
        {
            _settingsService = settingsService ??
                               throw new ArgumentNullException(nameof(settingsService));

            _navigationService = jellyfinNavigationService ??
                                 throw new ArgumentNullException(nameof(jellyfinNavigationService));

            IsShowMovies = true;
        }

        #endregion

        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Movies":
                    IsShowMovies = true;
                    IsShowTvShows = false;
                    IsShowMusic = false; 
                    break;
                case "TvShows":
                    IsShowMovies = false;
                    IsShowTvShows = true;
                    IsShowMusic = false;
                    break;
                case "Music":
                    IsShowMovies = false;
                    IsShowTvShows = false;
                    IsShowMusic = true;
                    break;
                case "Logout":
                    Logout();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        private void Logout()
        {
            _settingsService.Clear();
            _navigationService.Navigate(typeof(LoginView));
        }

        #endregion
    }
}