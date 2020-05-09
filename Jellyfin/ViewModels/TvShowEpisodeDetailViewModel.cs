using System;
using System.ComponentModel;
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

            PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Additional methods

        public override void Execute(string commandParameter)
        {
            switch (commandParameter)
            {
                case "Play":
                    Play();
                    break;
                case "PlayFromBeginning":
                    PlayFromBeginning(false);
                    break;
                default:
                    base.Execute(commandParameter);
                    break;
            }
        }

        public async Task ConfigureInitialTvShowDetails(MediaElementBase mediaElementBase)
        {
            SelectedMediaElement = mediaElementBase;
            await GetTvShowDetailsImpl();
        }

        private async Task GetTvShowDetailsImpl()
        {
            if (SelectedMediaElement == null)
            {
                return;
            }

            TvShowEpisode gotEpisode = (TvShowEpisode) SelectedMediaElement;

            if (!gotEpisode.Season.TvShowEpisodes.Any())
            {
                await _tvShowService.GetEpisodesBy(gotEpisode.TvShow, gotEpisode.Season);
            }
            else
            {
                foreach (TvShowEpisode episode in gotEpisode.Season.TvShowEpisodes)
                {
                    SelectedSeasonEpisodes.Add(episode);
                }
            }

            if (gotEpisode.PlaybackInformation == null)
            {
                gotEpisode.PlaybackInformation = await _playbackInfoService.GetPlaybackInformation(gotEpisode.Id);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedMediaElement))
            {
                GetTvShowDetailsImpl();
            }
        }

        private void Play()
        {
            if (SelectedMediaElement.PlaybackPosition != TimeSpan.Zero && SelectedMediaElement.PlaybackPosition.TotalMinutes > 2 && SelectedMediaElement.PlaybackRemaining.TotalMinutes > 2)
            {
                NavigationService.Navigate(typeof(PlaybackConfirmationView), new PlaybackViewParameterModel
                {
                    SelectedMediaElement = SelectedMediaElement,
                    NextMediaElement = GetNextMediaElement().Result
                });
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

        private async Task<MediaElementBase> GetNextMediaElement()
        {
            var episode = (TvShowEpisode) SelectedMediaElement;
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
                } else
                {
                    var nextSeason = seasons[seasonNumber + 1];
                    var nextSeasonEpisodes = await _tvShowService.GetEpisodesBy(episode.TvShow, nextSeason);

                    return nextSeasonEpisodes.FirstOrDefault();
                }
            }
            else
            {
                return episodes[epNumber + 1];
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