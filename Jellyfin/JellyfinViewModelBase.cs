﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Jellyfin.Services.Interfaces;
using Unity;

namespace Jellyfin.Core
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

        public virtual void Execute(string commandParameter)
        {
            //throw new NotImplementedException();
            // TODO smurancsik: log error
        }

        protected bool CanExecute(string commandParameter)
        {
            return true;
        }

        #endregion
    }
}
