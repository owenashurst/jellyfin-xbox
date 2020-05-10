using System;
using Jellyfin.Core;
using Jellyfin.Logging;
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

        private void ProcessTvShowImages(MediaElementBase mediaElementBase)
        {
            // TODO smurancsik: to fix up this workaround
            if (mediaElementBase.GetType() == typeof(TvShowEpisode))
            {
                if (!string.IsNullOrEmpty(mediaElementBase.BackdropImageId))
                {
                    mediaElementBase.BackdropImageData =
                        _imageService.GetImage(mediaElementBase.Id, mediaElementBase.BackdropImageId,
                            ImageTypeEnum.Primary).Result;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(mediaElementBase.ImageId))
                {
                    mediaElementBase.ImageData =
                        _imageService.GetImage(mediaElementBase.Id, mediaElementBase.ImageId, ImageTypeEnum.Primary)
                            .Result;
                }

                if (!string.IsNullOrEmpty(mediaElementBase.BackdropImageId))
                {
                    mediaElementBase.BackdropImageData =
                        _imageService.GetImage(mediaElementBase.Id, mediaElementBase.BackdropImageId,
                            ImageTypeEnum.Backdrop).Result;
                }
            }
        }

        #endregion
    }
}