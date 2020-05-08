using System;
using System.Text;
using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public abstract class NavigableMediaElementViewModelBase : JellyfinViewModelBase
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
                RaisePropertyChanged(nameof(FormattedResumeText));
            }
        }

        #endregion

        #region FormattedResumeText

        public string FormattedResumeText
        {
            get
            {
                if (SelectedMediaElement == null)
                {
                    return string.Empty;
                }

                StringBuilder bld = new StringBuilder();
                bld.Append("Resume from ");

                if (SelectedMediaElement.PlaybackPosition.Hours > 0)
                {
                    bld.Append(SelectedMediaElement.PlaybackPosition.Hours).Append(":");
                    bld.Append(SelectedMediaElement.PlaybackPosition.Minutes.ToString().PadLeft(2, '0')).Append(":");
                }
                else
                {
                    bld.Append(SelectedMediaElement.PlaybackPosition.Minutes).Append(":");
                }


                bld.Append(SelectedMediaElement.PlaybackPosition.Seconds.ToString().PadLeft(2, '0'));

                return bld.ToString();
            }
        }

        #endregion

        #endregion

        #region Additional methods

        public void NavigateToSelected()
        {
            Type view = null;
            if (SelectedMediaElement is Movie)
            {
                view = typeof(MovieDetailView);
            }
            else if (SelectedMediaElement is TvShowSeason)
            {
                view = typeof(TvShowDetailView);
            }
            else if (SelectedMediaElement is TvShowEpisode)
            {
                view = typeof(TvShowEpisodeDetailView);
            }
            else if (SelectedMediaElement is TvShow)
            {
                view = typeof(TvShowDetailView);
            }

            if (view != null)
            {
                NavigationService.Navigate(view, SelectedMediaElement);
            }
        }

        #endregion
    }
}