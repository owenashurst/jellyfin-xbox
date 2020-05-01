namespace Jellyfin.Models.ServiceModels.PlaybackInformation
{

    public class PlaybackInformationResult
    {
        public Mediasource[] MediaSources { get; set; }
        public string PlaySessionId { get; set; }
    }
}
