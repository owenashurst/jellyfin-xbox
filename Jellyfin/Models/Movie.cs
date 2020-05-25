using System;
using Jellyfin.Models.ServiceModels;

namespace Jellyfin.Models
{
    /// <summary>
    /// Movie model representation
    /// </summary>
    public class Movie : MediaElementBase
    {
        #region Properties

        public override string SecondLine
        {
            get => Year;
        }
        
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