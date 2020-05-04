using Jellyfin.Models;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public abstract class MediaElementListViewModelBase : JellyfinViewModelBase
    {
        #region Properties

        #region SelectedMediaElement

        private MediaElementBase _selectedMediaElement;

        public MediaElementBase SelectedMediaElement
        {
            get { return _selectedMediaElement; }
            set
            {
                _selectedMediaElement = value;
                RaisePropertyChanged(nameof(SelectedMediaElement));
            }
        }

        #endregion

        #endregion

        #region Additional methods

        public void NavigateToSelected()
        {
            if (SelectedMediaElement is Movie)
            {
                NavigationService.Navigate(typeof(MovieDetailView), SelectedMediaElement);
            }
            else if (SelectedMediaElement is TvShow)
            {
                NavigationService.Navigate(typeof(TvShowDetailView), SelectedMediaElement);
            }
        }

        #endregion
    }
}