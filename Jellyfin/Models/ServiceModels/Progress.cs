namespace Jellyfin.Models.ServiceModels
{

    public class Progress
    {
        public int AudioStreamIndex { get; set; }
        public Bufferedrange[] BufferedRanges { get; set; }
        public bool CanSeek { get; set; }
        public string EventName { get; set; }
        public bool IsMuted { get; set; }
        public bool IsPaused { get; set; }
        public string ItemId { get; set; }
        public int MaxStreamingBitrate { get; set; }
        public string MediaSourceId { get; set; }
        public string PlayMethod { get; set; }
        public string PlaySessionId { get; set; }
        public long PlaybackStartTimeTicks { get; set; }
        public string PlaylistItemId { get; set; }
        public int PositionTicks { get; set; }
        public string RepeatMode { get; set; }
        public int SubtitleStreamIndex { get; set; }
        public int VolumeLevel { get; set; }
    }

    public class Bufferedrange
    {
        public int end { get; set; }
        public int start { get; set; }
    }

}
