using System;

namespace Jellyfin.Models.ServiceReturnModels.Movie
{
    public class Userdata
    {
        public float PlayedPercentage { get; set; }
        public long PlaybackPositionTicks { get; set; }
        public int PlayCount { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime LastPlayedDate { get; set; }
        public bool Played { get; set; }
        public string Key { get; set; }
    }
}