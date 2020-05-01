using Jellyfin.Models.ServiceModels.PlaybackInformation;

namespace Jellyfin.Models.Adapters
{
    /// <summary>
    /// Adapter to map the JSON Movie Detail to Movie detail.
    /// </summary>
    public class MediaElementPlaybackSourceAdapter : IAdapter<Mediasource, MediaElementPlaybackSource>
    {
        #region Additional methods

        public MediaElementPlaybackSource Convert(Mediasource source)
        {
            MediaElementPlaybackSource m = new MediaElementPlaybackSource();

            m.Container = source.Container;
            m.TranscodingUrl = source.TranscodingUrl;

            return m;
        }

        #endregion


    }
}