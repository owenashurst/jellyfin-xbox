using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Jellyfin.Extensions;
using Jellyfin.Logging;
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

        public TvShowEpisodeDetailViewModel(ITvShowService tvShowService, IPlaybackInfoService playbackInfoService, IPersonalizeService personalizeService, ILogManager logManager) : base(personalizeService, logManager)
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Play();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
                    if (SelectedSeasonEpisodes.Any(q => q.Id == episode.Id))
                    {
                        continue;
                    }

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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                GetTvShowDetailsImpl();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        private async Task Play()
        {
            int currentSelectedNumber = ((TvShowEpisode) SelectedMediaElement).IndexNumber;
            TvShowEpisode[] remainingSeasonEpisodes =
                ((TvShowEpisode) SelectedMediaElement).Season.TvShowEpisodes
                    .Where(q => q.IndexNumber > currentSelectedNumber)
                    .OrderBy(q => q.IndexNumber).ToArray();

            NavigationService.Navigate(typeof(PlaybackConfirmationView), new PlaybackViewParameterModel
            {
                SelectedMediaElement = SelectedMediaElement,
                Playlist = remainingSeasonEpisodes
            });
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