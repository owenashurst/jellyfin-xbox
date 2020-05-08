using System;

namespace Jellyfin.Models
{
    public class TvShowEpisode : MediaElementBase
    {
        #region Properties

        public override string SecondLine
        {
            get { return $"{SeasonName}, Episode {IndexNumber}"; }
        }

        public TvShow TvShow { get; set; }

        public string SeriesName { get; set; }

        [Obsolete("Use TvShow.Id.")]
        public string SeriesId { get; set; }

        public TvShowSeason Season { get; set; }

        [Obsolete("Use Season.Id.")]
        public string SeasonId { get; set; }

        public string SeasonName { get; set; }

        public DateTime PremiereDate { get; set; }

        public int IndexNumber { get; set; }

        public TimeSpan PlayedLength { get; set; }

        #endregion
    }
}
