namespace Jellyfin.Models
{
    public class PlaybackViewParameterModel
    {
        #region Properties

        public MediaElementBase SelectedMediaElement { get; set; }

        public bool IsPlaybackFromBeginning { get; set; }

        public bool WasPlaybackPopupShown { get; set; }

        #endregion
    }
}
