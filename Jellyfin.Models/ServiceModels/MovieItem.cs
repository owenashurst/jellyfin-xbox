using System;

namespace Jellyfin.Models.ServiceModels
{
    public class MovieItem : Item
    {
        public MovieUserData UserData { get; set; }

        public bool HasSubtitles { get; set; }

        public string Container { get; set; }

        public DateTimeOffset PremiereDate { get; set; }

        public long? CriticRating { get; set; }

        public string VideoType { get; set; }

        public string[] Genres { get; set; }

        public string MediaType { get; set; }

        public string[] BackdropImageTags { get; set; }

    }
}