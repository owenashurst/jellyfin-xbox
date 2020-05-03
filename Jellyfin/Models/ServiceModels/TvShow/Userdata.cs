namespace Jellyfin.Models.ServiceModels.TvShow
{
    public class Userdata
    {
        public bool IsFavorite { get; set; }
        public string Key { get; set; }
        public int PlayCount { get; set; }
        public int PlaybackPositionTicks { get; set; }
        public bool Played { get; set; }
        public int UnplayedItemCount { get; set; }
    }

}
