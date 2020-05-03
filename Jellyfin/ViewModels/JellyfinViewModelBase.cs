using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;
using Unity;

namespace Jellyfin.ViewModels
{
    public abstract class JellyfinViewModelBase : ViewModelBase
    {
        #region Properties

        public string HashCode
        {
            get { return GetHashCode().ToString(); }
        }

        /// <summary>
        /// For accessing global params from UI
        /// </summary>
        public Globals AppGlobals
        {
            get { return Globals.Instance; }
        }

        #region ToolCommand

        private RelayCommand<string> _toolCommand;

        public RelayCommand<string> ToolCommand
        {
            get { return _toolCommand; }
            set { _toolCommand = value; }
        }

        #endregion

        #region IsLoading

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged(nameof(IsLoading));
                RaisePropertyChanged(nameof(LoadingText));
            }
        }

        #endregion


        #region LoadingText

        private string _loadingText = "Loading...";

        public string LoadingText
        {
            get { return _loadingText; }
            set
            {
                _loadingText = value;
                RaisePropertyChanged(nameof(LoadingText));
            }
        }

        #endregion

        protected IJellyfinNavigationService NavigationService { get; set; }   

        #endregion

        #region ctor

        protected JellyfinViewModelBase()
        {
            IUnityContainer container = Globals.Instance.Container;
            NavigationService = container.Resolve<IJellyfinNavigationService>();

            ToolCommand = new RelayCommand<string>(Execute, CanExecute);
        }

        #endregion

        #region Additional methods

        protected virtual void Execute(string commandParameter)
        {
            //throw new NotImplementedException();
        }

        protected bool CanExecute(string commandParameter)
        {
            return true;
        }

        public void NavigateToMovie(Movie movie)
        {
            NavigationService.Navigate(typeof(MovieDetailView), movie);
        }

        #endregion
    }
}
