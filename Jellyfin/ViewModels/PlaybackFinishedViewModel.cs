using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class PlaybackFinishedViewModel : NavigableMediaElementViewModelBase
    {
        #region Properties

        #region NextMediaElement

        private MediaElementBase _nextMediaElement;

        public MediaElementBase NextMediaElement
        {
            get { return _nextMediaElement; }
            set
            {
                _nextMediaElement = value;
                RaisePropertyChanged(nameof(NextMediaElement));
            }
        }

        #endregion

        #region NextAfterMediaElement

        private MediaElementBase _nextAfterMediaElement;

        public MediaElementBase NextAfterMediaElement
        {
            get { return _nextAfterMediaElement; }
            set
            {
                _nextAfterMediaElement = value;
                RaisePropertyChanged(nameof(NextAfterMediaElement));
            }
        }

        #endregion

        /// <summary>
        /// Reference for the tv show service, to retrieve the next element.
        /// </summary>
        private readonly ITvShowService _tvShowService;

        #endregion

        #region ctor

        public PlaybackFinishedViewModel(ITvShowService tvShowService)
        {
            _tvShowService = tvShowService ??
                             throw new ArgumentNullException(nameof(tvShowService));
        }

        #endregion

        #region Additional methods

        public override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "PlayNext":
                    PlayNext();
                    break;
                case "Replay":
                    Replay();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        public async Task Update(PlaybackViewParameterModel vpm)
        {
            SelectedMediaElement = vpm.SelectedMediaElement;
            NextMediaElement = vpm.NextMediaElement;
            NextAfterMediaElement = await GetNextMediaElement();
        }

        private async Task<MediaElementBase> GetNextMediaElement()
        {
            TvShowEpisode episode = (TvShowEpisode) SelectedMediaElement;
            ObservableCollectionEx<TvShowEpisode> episodes = episode.Season.TvShowEpisodes;

            if (episodes.IndexOf(episode) == episodes.Count)
            {
                TvShowSeason season = episode.Season;
                ObservableCollectionEx<TvShowSeason> seasons = episode.TvShow.Seasons;
                if (seasons.IndexOf(season) == seasons.Count)
                {
                    return null;
                }

                TvShowSeason nextSeason = seasons[seasons.IndexOf(season) + 1];
                IEnumerable<TvShowEpisode> nextSeasonEpisodes = await _tvShowService.GetEpisodesBy(episode.TvShow, nextSeason);

                return nextSeasonEpisodes.FirstOrDefault();
            }

            return episodes[episodes.IndexOf(episode) + 1];
        }

        private void PlayNext()
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = NextMediaElement,
                IsPlaybackFromBeginning = true,
                NextMediaElement = NextAfterMediaElement
            });
        }

        private void Replay()
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                IsPlaybackFromBeginning = false,
                WasPlaybackPopupShown = true,
                NextMediaElement = NextMediaElement
            });
        }

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