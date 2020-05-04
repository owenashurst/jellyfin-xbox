using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Jellyfin.Core;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Models.Adapters;
using Jellyfin.Models.ServiceModels.TvShow;
using Jellyfin.Services.Interfaces;
using Newtonsoft.Json;

namespace Jellyfin.Services
{
    public class TvShowService : ServiceBase, ITvShowService
    {
        #region Properties

        /// <summary>
        /// Queue for downloading tv show images.
        /// </summary>
        public TaskQueue<TvShow> TvShowImageDownloadQueue { get; set; }

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

        public string GetRelatedTvShowsEndpoint
        {
            get => $"{Globals.Instance.Host}/Items/{{0}}/Similar?userId={Globals.Instance.User.Id}&limit=12&fields=PrimaryImageAspectRatio";
        }

        /// <summary>
        /// Reference for the tv show adapter.
        /// </summary>
        private readonly IAdapter<Item, TvShow> _tvShowAdapter;

        /// <summary>
        /// Reference for the tv show details adapter.
        /// </summary>
        private readonly IAdapter<TvShowDetailsResult, TvShow> _tvShowDetailsAdapter;

        /// <summary>
        /// Reference for the image service.
        /// </summary>
        private readonly IImageService _imageService;

        #endregion

        #region ctor

        public TvShowService(IAdapter<Item, TvShow> tvShowAdapter,
            IAdapter<TvShowDetailsResult, TvShow> tvShowDetailsAdapter,
            IImageService imageService)
        {
            _tvShowAdapter = tvShowAdapter ??
                            throw new ArgumentNullException(nameof(tvShowAdapter));

            _tvShowDetailsAdapter = tvShowDetailsAdapter ??
                                   throw new ArgumentNullException(nameof(tvShowDetailsAdapter));

            _imageService = imageService ??
                            throw new ArgumentNullException(nameof(imageService));

            TvShowImageDownloadQueue = new TaskQueue<TvShow>(1, ProcessTvShowImages);
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
                
                foreach (Item item in resultSet.Items)
                {
                    TvShow tvShow = _tvShowAdapter.Convert(item);
                    tvShowList.Add(tvShow);
                    TvShowImageDownloadQueue.EnqueueTask(tvShow);
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

                    HttpResponseMessage result = cli.GetAsync($"{GetTvShowDetailsEndpoint}{tvShowId}").Result;

                    if (!result.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    string jsonResult = await result.Content.ReadAsStringAsync();

                    TvShowDetailsResult resultSet = JsonConvert.DeserializeObject<TvShowDetailsResult>(jsonResult);

                    TvShow item = _tvShowDetailsAdapter.Convert(resultSet);
                    TvShowImageDownloadQueue.EnqueueTask(item);
                    return item;
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

        private void ProcessTvShowImages(TvShow tvShow)
        {
            if (!string.IsNullOrEmpty(tvShow.ImageId))
            {
                tvShow.ImageData =
                    _imageService.GetImage(tvShow.Id, tvShow.ImageId, ImageTypeEnum.Primary).Result;
            }

            if (!string.IsNullOrEmpty(tvShow.BackdropImageId))
            {
                tvShow.BackdropImageData =
                    _imageService.GetImage(tvShow.Id, tvShow.BackdropImageId, ImageTypeEnum.Backdrop).Result;
            }
        }

        #endregion
    }
}