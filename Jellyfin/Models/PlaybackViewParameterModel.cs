using System;
using System.Linq;

namespace Jellyfin.Models
{
    public class PlaybackViewParameterModel
    {
        #region Properties

        public MediaElementBase SelectedMediaElement { get; set; }

        public bool IsPlaybackFromBeginning { get; set; }

        public bool IsInvalidated { get; set; }

        /// <summary>
        /// Indicates whether the playback is just returned from playing.
        /// </summary>
        public bool IsJustFinishedPlaying { get; set; }

        /// <summary>
        /// The playlist of the upcoming items.
        /// </summary>
        public MediaElementBase[] Playlist { get; set; }

        #endregion

        #region Overrides

        public override string ToString()
        {
            string playlist = string.Empty;

            if (Playlist != null)
            {
                playlist = string.Join("/", Playlist.ToList());
            }

            return $"<{SelectedMediaElement}> IsPlaybackFromBeginning = {IsPlaybackFromBeginning}, " +
                   $"IsJustFinishedPlaying = {IsJustFinishedPlaying}, " +
                   $"Playlist = {playlist}";
        }

        #endregion
    }
}
