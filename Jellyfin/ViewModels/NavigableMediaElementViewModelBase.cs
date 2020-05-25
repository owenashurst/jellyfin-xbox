using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jellyfin.Core;
using Jellyfin.Logging;
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

        /// <summary>
        /// Reference for the personalize service.
        /// </summary>
        protected readonly IPersonalizeService _personalizeService;

        /// <summary>
        /// Reference for the log manager.
        /// </summary>
        protected readonly ILogManager _logManager;

        #endregion

        #region ctor

        protected NavigableMediaElementViewModelBase(IPersonalizeService personalizeService, ILogManager logManager)
        {
            _personalizeService = personalizeService ??
                    throw new ArgumentNullException(nameof(personalizeService));

            _logManager = logManager ??
                    throw new ArgumentNullException(nameof(logManager));
        }

        #endregion

        #region Additional methods

        public override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "MarkAsUnwatched":
                    MarkAsUnwatched();
                    break;
                case "MarkAsWatched":
                    MarkAsWatched();
                    break;
                case "Like":
                    Like();
                    break;
                case "Dislike":
                    Dislike();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

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

            return episodes[epNumber + 1];
        }

        /// <summary>
        /// Marks this media element as watched.
        /// </summary>
        /// <returns></returns>
        protected async Task MarkAsWatched()
        {
            if (SelectedMediaElement == null)
            {
                return;
            }

            try
            {
                await _personalizeService.MarkAsWatched(SelectedMediaElement.Id);

                if (SelectedMediaElement.UserData != null)
                {
                    SelectedMediaElement.UserData.Played = true;
                }
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, "An error occurred while marking the media element as watched.");
            }
        }

        // <summary>
        /// Marks this media element as unwatched.
        /// </summary>
        /// <returns></returns>
        protected async Task MarkAsUnwatched()
        {
            if (SelectedMediaElement == null)
            {
                return;
            }

            try
            {
                await _personalizeService.MarkAsUnwatched(SelectedMediaElement.Id);

                if (SelectedMediaElement.UserData != null)
                {
                    SelectedMediaElement.UserData.Played = false;
                }
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, "An error occurred while marking the media element as unwatched.");
            }
        }

        /// <summary>
        /// Likes this media element.
        /// </summary>
        /// <returns></returns>
        protected async Task Like()
        {
            if (SelectedMediaElement == null)
            {
                return;
            }

            try
            {
                await _personalizeService.Like(SelectedMediaElement.Id);

                if (SelectedMediaElement.UserData != null)
                {
                    SelectedMediaElement.UserData.IsFavorite = true;
                }
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, "An error occurred while marking the media element as liked.");
            }
        }

        /// <summary>
        /// Dislikes this media element.
        /// </summary>
        /// <returns></returns>
        protected async Task Dislike()
        {
            if (SelectedMediaElement == null)
            {
                return;
            }

            try
            {
                await _personalizeService.Dislike(SelectedMediaElement.Id);
                
                if (SelectedMediaElement.UserData != null)
                {
                    SelectedMediaElement.UserData.IsFavorite = false;
                }
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, "An error occurred while marking the media element as disliked.");
            }
        }

        #endregion
    }
}