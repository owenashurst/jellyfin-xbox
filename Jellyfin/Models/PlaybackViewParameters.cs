namespace Jellyfin.Models
{
    public class PlaybackViewParameters
    {
        #region Properties

        public Movie SelectedMovie { get; set; }

        public bool IsPlaybackFromBeginning { get; set; }

        #endregion
    }
}
