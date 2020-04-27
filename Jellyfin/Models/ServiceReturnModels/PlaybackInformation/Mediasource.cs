using Jellyfin.Models.ServiceReturnModels.Movie;

namespace Jellyfin.Models.ServiceReturnModels.PlaybackInformation
{
    public class Mediasource
    {
        public int Bitrate { get; set; }
        public string Container { get; set; }
        public int DefaultAudioStreamIndex { get; set; }
        public int DefaultSubtitleStreamIndex { get; set; }
        public string ETag { get; set; }
        public object[] Formats { get; set; }
        public bool GenPtsInput { get; set; }
        public string Id { get; set; }
        public bool IgnoreDts { get; set; }
        public bool IgnoreIndex { get; set; }
        public bool IsInfiniteStream { get; set; }
        public bool IsRemote { get; set; }
        public object[] MediaAttachments { get; set; }
        public Mediastream[] MediaStreams { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Protocol { get; set; }
        public bool ReadAtNativeFramerate { get; set; }
        public Requiredhttpheaders RequiredHttpHeaders { get; set; }
        public bool RequiresClosing { get; set; }
        public bool RequiresLooping { get; set; }
        public bool RequiresOpening { get; set; }
        public long RunTimeTicks { get; set; }
        public long Size { get; set; }
        public bool SupportsDirectPlay { get; set; }
        public bool SupportsDirectStream { get; set; }
        public bool SupportsProbing { get; set; }
        public bool SupportsTranscoding { get; set; }
        public string TranscodingContainer { get; set; }
        public string TranscodingSubProtocol { get; set; }
        public string TranscodingUrl { get; set; }
        public string Type { get; set; }
        public string VideoType { get; set; }
    }
}