using System;

namespace Jellyfin.Models.ServiceModels
{
    public class TvShowEpisodeItem : Item
    {
        public string HasSubtitles { get; set; }

        public string Container { get; set; }

        public DateTime PremiereDate { get; set; }

        public string Overview { get; set; }

        public int IndexNumber { get; set; }

        public string ParentBackdropItemId { get; set; }

        public string[] ParentBackdropImageTags { get; set; }

        public UserData UserData { get; set; }

        public string SeriesName { get; set; }

        public string SeriesId { get; set; }

        public string SeasonId { get; set; }

        public string SeasonName { get; set; }
    }
}