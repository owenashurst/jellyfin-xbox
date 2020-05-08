using System;
using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.Services.Interfaces;

namespace Jellyfin.Services
{
    public abstract class MediaQueryServiceBase : ServiceBase
    {
        #region Properties

        /// <summary>
        /// Reference for the image service.
        /// </summary>
        private readonly IImageService _imageService;

        /// <summary>
        /// Queue for media element images.
        /// </summary>
        public TaskQueue<MediaElementBase> ImageDownloadQueue { get; set; }

        #endregion

        #region ctor

        protected MediaQueryServiceBase(IImageService imageService)
        {
            _imageService = imageService ??
                            throw new ArgumentNullException(nameof(imageService));

            ImageDownloadQueue = new TaskQueue<MediaElementBase>(1, ProcessTvShowImages);
        }

        #endregion

        #region Additional methods

        private void ProcessTvShowImages(MediaElementBase tvShow)
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