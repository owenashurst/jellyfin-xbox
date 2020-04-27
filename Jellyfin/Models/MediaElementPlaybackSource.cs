namespace Jellyfin.Models
{
    public class MediaElementPlaybackSource : ModelBase
    {
        #region Properties

        public string Container { get; set; }

        public string TranscodingUrl { get; set; }

        #endregion
    }
}
