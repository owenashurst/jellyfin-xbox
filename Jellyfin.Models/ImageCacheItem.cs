using Jellyfin.Core.Models;

namespace Jellyfin.Models
{
    public class ImageCacheItem : ModelBase
    {
        #region Properties

        public string Id { get; set; }

        public string MediaElementId { get; set; }

        public string ImageType { get; set; }

        public byte[] Image { get; set; }

        #endregion
    }
}
