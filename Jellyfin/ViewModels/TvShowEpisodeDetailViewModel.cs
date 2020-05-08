using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Jellyfin.Views;

namespace Jellyfin.ViewModels
{
    public class TvShowEpisodeDetailViewModel : NavigableMediaElementViewModelBase
    {
        #region Properties

        /// <summary>
        /// Reference for the movie service.
        /// </summary>
        private readonly ITvShowService _tvShowService;

        /// <summary>
        /// Reference for the playback info service.
        /// </summary>
        private readonly IPlaybackInfoService _playbackInfoService;

        #region SelectedSeasonEpisodes

        private ObservableCollectionEx<TvShowEpisode> _selectedSeasonEpisodes = new ObservableCollectionEx<TvShowEpisode>();

        public ObservableCollectionEx<TvShowEpisode> SelectedSeasonEpisodes
        {
            get { return _selectedSeasonEpisodes; }
            set
            {
                _selectedSeasonEpisodes = value;
                RaisePropertyChanged(nameof(SelectedSeasonEpisodes));
            }
        }

        #endregion

        #endregion

        #region ctor

        public TvShowEpisodeDetailViewModel(ITvShowService tvShowService, IPlaybackInfoService playbackInfoService)
        {
            _tvShowService = tvShowService ??
                             throw new ArgumentNullException(nameof(tvShowService));

            _playbackInfoService = playbackInfoService ??
                                   throw new ArgumentNullException(nameof(tvShowService));
        }

        #endregion

        #region Additional methods

        protected override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Play":
                    Play();
                    break;
                case "PlayFromBeginning":
                    PlayFromBeginning(false);
                    break;
                case "PlayFromPosition":
                    PlayFromPosition();
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        public async Task GetTvShowDetails(MediaElementBase mediaElementBase)
        {
            SelectedMediaElement = mediaElementBase;

            TvShowEpisode gotEpisode = (TvShowEpisode) SelectedMediaElement;
            foreach (TvShowEpisode episode in
                await _tvShowService.GetEpisodesBy(gotEpisode.TvShow, gotEpisode.Season))
            {
                SelectedSeasonEpisodes.Add(episode);
            }

            if (gotEpisode.PlaybackInformation == null)
            {
                gotEpisode.PlaybackInformation = await _playbackInfoService.GetPlaybackInformation(gotEpisode.Id);
            }
        }

        private void Play()
        {
            if (SelectedMediaElement.PlaybackPosition != TimeSpan.Zero && SelectedMediaElement.PlaybackPosition.TotalMinutes > 2 && SelectedMediaElement.PlaybackRemaining.TotalMinutes > 2)
            {
                NavigationService.Navigate(typeof(PlaybackConfirmationView), SelectedMediaElement);
            }
            else
            {
                PlayFromBeginning(false);
            }
        }

        private void PlayFromBeginning(bool isPopupDisplayed)
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                IsPlaybackFromBeginning = true,
                WasPlaybackPopupShown = isPopupDisplayed,
                NextMediaElement = GetNextMediaElement().Result
            });
        }

        private void PlayFromPosition()
        {
            NavigationService.Navigate(typeof(MediaPlaybackView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                IsPlaybackFromBeginning = false,
                WasPlaybackPopupShown = true,
                NextMediaElement = GetNextMediaElement().Result
            });
        }

        private async Task<MediaElementBase> GetNextMediaElement()
        {
            var episode = (TvShowEpisode) SelectedMediaElement;
            var episodes = episode.Season.TvShowEpisodes;

            if (episodes.IndexOf(episode) == episodes.Count)
            {
                var season = episode.Season;
                var seasons = episode.TvShow.Seasons;
                if (seasons.IndexOf(season) == seasons.Count)
                {
                    return null;
                } else
                {
                    var nextSeason = seasons[seasons.IndexOf(season) + 1];
                    var nextSeasonEpisodes = await _tvShowService.GetEpisodesBy(episode.TvShow, nextSeason);

                    return nextSeasonEpisodes.FirstOrDefault();
                }
            }
            else
            {
                return episodes[episodes.IndexOf(episode) + 1];
            }
        }

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

        #endregion
    }
}