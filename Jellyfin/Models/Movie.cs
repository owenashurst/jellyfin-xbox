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
        /// Indicates whether the media element has subtitles.
        /// </summary>
        public bool HasSubtitles { get; set; }


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