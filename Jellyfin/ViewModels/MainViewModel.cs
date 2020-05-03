using System;
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
        
        #region IsMoviesPanelDisplayed

        private bool _isMoviesPanelDisplayed;

        public bool IsMoviesPanelDisplayed
        {
            get { return _isMoviesPanelDisplayed; }
            set
            {
                _isMoviesPanelDisplayed = value;
                RaisePropertyChanged(nameof(IsMoviesPanelDisplayed));
            }
        }

        #endregion

        #region IsTvShowsPanelDisplayed

        private bool _isTvShowsPanelDisplayed;

        public bool IsTvShowsPanelDisplayed
        {
            get { return _isTvShowsPanelDisplayed; }
            set
            {
                _isTvShowsPanelDisplayed = value;
                RaisePropertyChanged(nameof(IsTvShowsPanelDisplayed));
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

            IsMoviesPanelDisplayed = false;
            IsTvShowsPanelDisplayed = false;
        }

        #endregion

        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Movies":
                    IsTvShowsPanelDisplayed = false;
                    IsMoviesPanelDisplayed = true;
                    break;
                case "TvShows":
                    IsTvShowsPanelDisplayed = true;
                    IsMoviesPanelDisplayed = false;
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