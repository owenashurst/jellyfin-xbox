using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;
using Newtonsoft.Json;

namespace Jellyfin.Services
{
    public class ImageService : ServiceBase, IImageService
    {
        #region Properties

        private readonly ISettingsService _settingsService;

        private bool isCacheEnabled = false;

        private List<ImageCacheItem> ImageCache;

        public string GetImageEndpoint
        {
            get
            {
                return
                    $"{Globals.Instance.Host}/Items/{{0}}/Images/{{1}}?maxHeight=300&maxWidth=250&tag={{2}}&quality=90";
            }
        }

        #endregion

        #region ctor

        public ImageService(ISettingsService settingsService)
        {
            _settingsService = settingsService ??
                               throw new ArgumentNullException(nameof(settingsService));


            if (!isCacheEnabled)
            {
                return;
            }

            string imageCacheJson = _settingsService.GetProperty<string>("ImageCache");

            try
            {
                ImageCache = JsonConvert.DeserializeObject<List<ImageCacheItem>>(imageCacheJson);

                if (ImageCache == null)
                {
                    ImageCache = new List<ImageCacheItem>();
                    SaveImageCache();
                }
            }
            catch (Exception xc)
            {
                ImageCache = new List<ImageCacheItem>();
                SaveImageCache();
            }
        }

        #endregion

        #region Additional methods

        /// <summary>
        /// Retrieves image as byte array by its id.
        /// </summary>
        /// <param name="id">The ID of the media library element.</param>
        /// <param name="imageId">The ID of the image.</param>
        /// <returns></returns>
        public async Task<byte[]> GetImage(string id, string imageId, ImageTypeEnum imageType)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (string.IsNullOrEmpty(imageId))
            {
                return null;
            }

            if (isCacheEnabled)
            {
                ImageCacheItem image = ImageCache.FirstOrDefault(q =>
                    q.Id == imageId && q.ImageType == imageType.ToString() && q.MediaElementId == id);
                if (image != null)
                {
                    return image.Image;
                }
            }

            string imageEndpoint = string.Format(GetImageEndpoint, id, imageType, imageId);

            using (HttpClient cli = new HttpClient())
            {
                cli.AddAuthorizationHeaders();

                HttpResponseMessage result = await cli.GetAsync(imageEndpoint);
                byte[] retrievedImage = await result.Content.ReadAsByteArrayAsync();

                if (isCacheEnabled)
                {
                    ImageCacheItem cacheItem = new ImageCacheItem
                    {
                        Id = imageId,
                        Image = retrievedImage,
                        ImageType = imageType.ToString(),
                        MediaElementId = id
                    };

                    ImageCache.Add(cacheItem);
                    SaveImageCache();
                }

                return retrievedImage;
            }
        }

        private void SaveImageCache()
        {
            string json = JsonConvert.SerializeObject(ImageCache);
            _settingsService.SetProperty("ImageCache", json);
        }

        #endregion
    }
}