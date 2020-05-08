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

        public string SeriesName { get; set; }

        public string SeriesId { get; set; }

        public string SeasonId { get; set; }

        public string SeasonName { get; set; }

        public DateTime PremiereDate { get; set; }

        public int IndexNumber { get; set; }

        public TimeSpan PlayedLength { get; set; }

        #endregion
    }
}
