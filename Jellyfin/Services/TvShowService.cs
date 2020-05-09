using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Jellyfin.Core;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Models.ServiceModels;
using Jellyfin.Models.ServiceModels.TvShow;
using Jellyfin.Services.Interfaces;
using Newtonsoft.Json;

namespace Jellyfin.Services
{
    public class TvShowService : MediaQueryServiceBase, ITvShowService
    {
        #region Properties

        public string ListTvShowsEndpoint
        {
            get =>
                $"{Globals.Instance.Host}/Users/{Globals.Instance.User.Id}/Items?IncludeItemTypes=Series&Recursive=true&Fields=PrimaryImageAspectRatio%2CMediaSourceCount%2CBasicSyncInfo%2CGenres&ImageTypeLimit=1&EnableImageTypes=Primary%2CBackdrop%2CBanner%2CThumb&StartIndex=0&Limit=100000"
            ;
        }

        public string GetTvShowDetailsEndpoint
        {
            get => $"{Globals.Instance.Host}/Users/{Globals.Instance.User.Id}/Items/";
        }

        public string GetTvShowSeasonsByTvShowIdEndpoint   
        {
            get => $"{Globals.Instance.Host}/Shows/{{0}}/Seasons?userId={Globals.Instance.User.Id}&Fields=ItemCounts%2CPrimaryImageAspectRatio%2CBasicSyncInfo%2CCanDelete%2CMediaSourceCount";
        }

        public string GetTvShowEpisodesBySeasonEndpoint
        {
            get => $"{Globals.Instance.Host}/Shows/{{0}}/Episodes?seasonId={{1}}&userId={Globals.Instance.User.Id}&Fields=ItemCounts%2CPrimaryImageAspectRatio%2CBasicSyncInfo%2CCanDelete%2CMediaSourceCount%2COverview";
        }

        public string GetContinueWatchingEpisodesEndpoint
        {
            get
            {
                return
                    $"{Globals.Instance.Host}/Users/{Globals.Instance.User.Id}/Items?SortBy=DatePlayed&SortOrder=Descending&IncludeItemTypes=Episode&Filters=IsResumable&Limit=6&Recursive=true&Fields=PrimaryImageAspectRatio%2CSeriesInfo%2COverview%2CUserData%2CBasicSyncInfo&ExcludeLocationTypes=Virtual&ImageTypeLimit=1&EnableImageTypes=Primary%2CBackdrop%2CThumb&EnableTotalRecordCount=false";
            }
        }

        public string GetRelatedTvShowsEndpoint
        {
            get => $"{Globals.Instance.Host}/Items/{{0}}/Similar?userId={Globals.Instance.User.Id}&limit=12&fields=PrimaryImageAspectRatio";
        }

        /// <summary>
        /// Reference for the tv show adapter.
        /// </summary>
        private readonly IAdapter<TvShowItem, TvShow> _tvShowAdapter;

        /// <summary>
        /// Reference for the tv show details adapter.
        /// </summary>
        private readonly IAdapter<TvShowDetailsResult, TvShow> _tvShowDetailsAdapter;

        /// <summary>
        /// Reference for the tv show season adapter.
        /// </summary>
        private readonly IAdapter<TvShowSeasonItem, TvShowSeason> _tvShowSeasonAdapter;

        /// <summary>
        /// Reference for the tv show episode adapter.
        /// </summary>
        private readonly IAdapter<TvShowEpisodeItem, TvShowEpisode> _tvShowEpisodeAdapter;

        #endregion

        #region ctor

        public TvShowService(IAdapter<TvShowItem, TvShow> tvShowAdapter,
            IAdapter<TvShowDetailsResult, TvShow> tvShowDetailsAdapter,
            IAdapter<TvShowSeasonItem, TvShowSeason> tvShowSeasonAdapter,
            IAdapter<TvShowEpisodeItem, TvShowEpisode> tvShowEpisodeAdapter,
            IImageService imageService) : base(imageService)
        {
            _tvShowAdapter = tvShowAdapter ??
                            throw new ArgumentNullException(nameof(tvShowAdapter));

            _tvShowDetailsAdapter = tvShowDetailsAdapter ??
                                   throw new ArgumentNullException(nameof(tvShowDetailsAdapter));

            _tvShowSeasonAdapter = tvShowSeasonAdapter ??
                                   throw new ArgumentNullException(nameof(tvShowSeasonAdapter));

            _tvShowEpisodeAdapter = tvShowEpisodeAdapter ??
                                    throw new ArgumentNullException(nameof(tvShowEpisodeAdapter));
        }
        
        #endregion

        #region Additional methods

        public async Task<IEnumerable<TvShow>> GetTvShows()
        {
            List<TvShow> tvShowList = new List<TvShow>();

            using (HttpClient cli = new HttpClient())
            {
                cli.AddAuthorizationHeaders();

                HttpResponseMessage result = await cli.GetAsync(ListTvShowsEndpoint);

                if (!result.IsSuccessStatusCode)
                {
                    return new List<TvShow>();
                }

                string jsonResult = await result.Content.ReadAsStringAsync();

                JellyfinTvShowResult resultSet = JsonConvert.DeserializeObject<JellyfinTvShowResult>(jsonResult);
                
                foreach (TvShowItem item in resultSet.Items)
                {
                    TvShow tvShow = _tvShowAdapter.Convert(item);
                    tvShowList.Add(tvShow);
                    ImageDownloadQueue.EnqueueTask(tvShow);
                }
            }

            return tvShowList;
        }

        public async Task<TvShow> GetTvShowDetails(string tvShowId)
        {
            try
            {
                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    HttpResponseMessage result = await cli.GetAsync($"{GetTvShowDetailsEndpoint}{tvShowId}");

                    if (!result.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    string jsonResult = await result.Content.ReadAsStringAsync();

                    TvShowDetailsResult resultSet = JsonConvert.DeserializeObject<TvShowDetailsResult>(jsonResult);

                    TvShow item = _tvShowDetailsAdapter.Convert(resultSet);
                    ImageDownloadQueue.EnqueueTask(item);
                    return item;
                }
            }
            catch (Exception xc)
            {
                // TODO smurancsik add correct logging
            }

            return null;
        }

        // todo smurancsik: validate input.
        public async Task<IEnumerable<TvShowSeason>> GetSeasonsBy(TvShow tvShow)
        {
            IList<TvShowSeason> seasons = new List<TvShowSeason>();

            try
            {
                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    HttpResponseMessage result = await cli.GetAsync(string.Format(GetTvShowSeasonsByTvShowIdEndpoint, tvShow.Id));

                    if (!result.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    string jsonResult = await result.Content.ReadAsStringAsync();

                    TvShowSeasonResult resultSet = JsonConvert.DeserializeObject<TvShowSeasonResult>(jsonResult);

                    foreach (TvShowSeasonItem item in resultSet.Items)
                    {
                        TvShowSeason tvShowSeason = _tvShowSeasonAdapter.Convert(item);
                        seasons.Add(tvShowSeason);

                        tvShowSeason.TvShow = tvShow;

                        if (tvShow.Seasons.All(q => q.Id != tvShowSeason.Id))
                        {
                            tvShow.Seasons.Add(tvShowSeason);
                        }

                        ImageDownloadQueue.EnqueueTask(tvShowSeason);
                    }

                    return seasons;
                }
            }
            catch (Exception xc)
            {
                // TODO smurancsik add correct logging
            }

            return null;
        }

        public async Task<IEnumerable<TvShowEpisode>> GetEpisodesBy(TvShow tvShow, TvShowSeason season)
        {
            IList<TvShowEpisode> episodes = new List<TvShowEpisode>();

            try
            {
                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    HttpResponseMessage result = await cli.GetAsync(string.Format(GetTvShowEpisodesBySeasonEndpoint, tvShow.Id, season.Id));

                    if (!result.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    string jsonResult = await result.Content.ReadAsStringAsync();

                    TvShowEpisodeResult resultSet = JsonConvert.DeserializeObject<TvShowEpisodeResult>(jsonResult);

                    foreach (TvShowEpisodeItem item in resultSet.Items)
                    {
                        TvShowEpisode tvShowEpisode = _tvShowEpisodeAdapter.Convert(item);
                        episodes.Add(tvShowEpisode);

                        tvShowEpisode.Season = season;
                        tvShowEpisode.TvShow = tvShow;

                        if (season.TvShowEpisodes.All(q => q.Id != tvShowEpisode.Id))
                        {
                            season.TvShowEpisodes.Add(tvShowEpisode);
                        }

                        ImageDownloadQueue.EnqueueTask(tvShowEpisode);
                    }

                    return episodes;
                }
            }
            catch (Exception xc)
            {
                // TODO smurancsik add correct logging
            }

            return null;
        }

        public async Task<IEnumerable<TvShowEpisode>> GetContinueWatchingEpisodes()
        {
            IList<TvShowEpisode> episodes = new List<TvShowEpisode>();

            try
            {
                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    HttpResponseMessage result = await cli.GetAsync(GetContinueWatchingEpisodesEndpoint);

                    if (!result.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    string jsonResult = await result.Content.ReadAsStringAsync();

                    TvShowEpisodeResult resultSet = JsonConvert.DeserializeObject<TvShowEpisodeResult>(jsonResult);

                    foreach (TvShowEpisodeItem item in resultSet.Items)
                    {
                        TvShowEpisode tvShowEpisode = _tvShowEpisodeAdapter.Convert(item);
                        episodes.Add(tvShowEpisode);

                        //tvShowEpisode.Season = season;
                        //tvShowEpisode.TvShow = tvShow;

                        //if (season.TvShowEpisodes.All(q => q.Id != tvShowEpisode.Id))
                        //{
                        //    season.TvShowEpisodes.Add(tvShowEpisode);
                        //}

                        ImageDownloadQueue.EnqueueTask(tvShowEpisode);
                    }

                    return episodes;
                }
            }
            catch (Exception xc)
            {
                // TODO smurancsik add correct logging
            }

            return null;
        }

        public async Task<IEnumerable<TvShow>> GetRelatedTvShows(string tvShowId)
        {
            //List<TvShow> tvShowList = new List<TvShow>();

            //using (HttpClient cli = new HttpClient())
            //{
            //    cli.AddAuthorizationHeaders();

            //    HttpResponseMessage result = await cli.GetAsync(string.Format(GetRelatedTvShowsEndpoint, tvShowId));

            //    if (!result.IsSuccessStatusCode)
            //    {
            //        return new List<TvShow>();
            //    }

            //    string jsonResult = await result.Content.ReadAsStringAsync();

            //    JellyfinTvShowResult resultSet = JsonConvert.DeserializeObject<JellyfinTvShowResult>(jsonResult);

            //    foreach (Item item in resultSet.Items)
            //    {
            //        TvShow tvShow = _tvShowAdapter.Convert(item);
            //        tvShowList.Add(tvShow);
            //        TvShowImageDownloadQueue.EnqueueTask(tvShow);
            //    }
            //}

            //return tvShowList;
            return null;
        }

        

        #endregion
    }
}