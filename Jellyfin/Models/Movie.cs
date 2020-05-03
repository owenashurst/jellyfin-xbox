using System;

namespace Jellyfin.Models
{
    /// <summary>
    /// Movie model representation
    /// </summary>
    public class Movie : MediaElementBase
    {
        #region Properties
        
        /// <summary>
        /// The year of the release.
        /// See field ProductionYear
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Inidicates whether the media element has subtitles.
        /// </summary>
        public bool HasSubtitles { get; set; }

        /// <summary>
        /// The genres of the movie.
        /// </summary>
        public string[] Genres { get; set; }

        public string FormattedGenres
        {
            get => string.Join(", ", Genres);
        }

        /// <summary>
        /// The date when it went to premiere.
        /// </summary>
        public DateTimeOffset PremiereDate { get; set; }
        
        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"<{Id}> {Name}";
        }

        #endregion
    }
}