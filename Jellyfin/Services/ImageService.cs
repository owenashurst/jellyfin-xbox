using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Jellyfin.Extensions;
using Jellyfin.Logging;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;

namespace Jellyfin.Services
{
    public class ImageService : ServiceBase, IImageService
    {
        #region Properties

        private readonly ILocalCacheService _localCacheService;

        private bool isCacheEnabled = true;

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

        public ImageService(ILocalCacheService localCacheService)
        {
            _localCacheService = localCacheService ??
                throw new ArgumentNullException(nameof(localCacheService));
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

            ImageCacheItem cacheItem = new ImageCacheItem
            {
                Id = imageId,
                ImageType = imageType.ToString(),
                MediaElementId = id
            };

            if (isCacheEnabled)
            {
                ImageCacheItem retrievedItem = await _localCacheService.Get<ImageCacheItem>(cacheItem.GetFileName());

                if (retrievedItem != null)
                {
                    return retrievedItem.Image;
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
                    cacheItem.Image = retrievedImage;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _localCacheService.Set(cacheItem.GetFileName(), cacheItem);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }

                return retrievedImage;
            }
        }

        #endregion
    }
}