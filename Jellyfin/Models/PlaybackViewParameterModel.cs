namespace Jellyfin.Models
{
    public class PlaybackViewParameterModel
    {
        #region Properties

        public Movie SelectedMovie { get; set; }

        public bool IsPlaybackFromBeginning { get; set; }

        public bool WasPlaybackPopupShown { get; set; }

        #endregion
    }
}
