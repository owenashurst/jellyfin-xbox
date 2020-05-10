using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;
using Unity;

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

        protected async Task<MediaElementBase> GetNextMediaElement(TvShowEpisode episode)
        {
            var episodes = episode.Season.TvShowEpisodes.OrderBy(q => q.IndexNumber).ToList();

            int epNumber = 0;
            for (; epNumber < episode.Season.TvShowEpisodes.Count; epNumber++)
            {
                TvShowEpisode episodeCheck = episode.Season.TvShowEpisodes[epNumber];
                if (episode.Id == episodeCheck.Id)
                {
                    break;
                }
            }

            if (epNumber == episodes.Count)
            {
                var season = episode.Season;
                var seasons = episode.TvShow.Seasons;

                int seasonNumber = 0;
                for (; seasonNumber < episode.TvShow.Seasons.Count; seasonNumber++)
                {
                    TvShowSeason seasonCheck = episode.TvShow.Seasons[seasonNumber];
                    if (season.Id == seasonCheck.Id)
                    {
                        break;
                    }
                }

                if (seasonNumber == seasons.Count)
                {
                    return null;
                }

                var nextSeason = seasons[seasonNumber + 1];
                IUnityContainer container = Globals.Instance.Container;
                var tvShowService = container.Resolve<ITvShowService>();
                var nextSeasonEpisodes = await tvShowService.GetEpisodesBy(episode.TvShow, nextSeason);

                return nextSeasonEpisodes.FirstOrDefault();
            }
            else
            {
                return episodes[epNumber + 1];
            }
        }

        #endregion
    }
}