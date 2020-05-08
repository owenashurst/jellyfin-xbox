using System;
using Jellyfin.Core.Models;

namespace Jellyfin.Models
{
    public class TvShowEpisode : ModelBase
    {
        #region Properties

        public string SeasonNumber { get; set; }

        public string EpisodeNumber { get; set; }

        public string EpisodeName { get; set; }

        public bool IsPlayed { get; set; }

        public TimeSpan Runtime { get; set; }

        public TimeSpan PlayedLength { get; set; }

        #endregion
    }
}
