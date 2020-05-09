using Jellyfin.Core.Models;

namespace Jellyfin.Models
{
    public class ImageCacheItem : ModelBase
    {
        #region Properties

        public string Id { get; set; }

        public string MediaElementId { get; set; }

        public string ImageType { get; set; }

        public int MaxHeight { get; set; }

        public int MaxWidth { get; set; }

        public byte[] Image { get; set; }

        #endregion

        #region ctor

        public ImageCacheItem()
        {
            MaxHeight = 300;
            MaxWidth = 250;
        }

        #endregion

        #region Additional methods

        public string GetFileName()
        {
            return $"{Id}_{MediaElementId}_{ImageType}_{MaxHeight}_{MaxWidth}.jpg";
        }

        #endregion
    }
}
