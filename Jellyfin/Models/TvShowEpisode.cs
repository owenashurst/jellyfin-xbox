using System;

namespace Jellyfin.Models
{
    public class TvShowEpisode : MediaElementBase
    {
        #region Properties

        public override string SecondLine
        {
            get { return $"S{SeasonNumber} E{IndexNumber}"; }
        }

        public TvShow TvShow { get; set; }

        public string SeriesName { get; set; }

        /// <summary>
        /// The ID of the tv show.
        /// Used for linking this episode with the tv show when the recommendations
        /// are downloaded.
        /// </summary>
        public string SeriesId { get; set; }

        public TvShowSeason Season { get; set; }

        /// <summary>
        /// The ID of the season.
        /// Used for linking this episode with the season when the recommendations
        /// are downloaded.
        /// </summary>
        public string SeasonId { get; set; }

        public string SeasonName { get; set; }

        public string SeasonNumber  { get; set; }

        public DateTime PremiereDate { get; set; }

        public int IndexNumber { get; set; }

        public TimeSpan PlayedLength { get; set; }

        public bool IsPlayed { get; set; }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"<{Id}> {Name} <- {SeriesName}, {SeasonName}, E{IndexNumber}";
        }

        #endregion
    }
}
