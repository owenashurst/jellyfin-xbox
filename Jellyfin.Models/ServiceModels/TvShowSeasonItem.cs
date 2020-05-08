namespace Jellyfin.Models.ServiceModels
{
    public class TvShowSeasonItem : Item
    {
        public string SeriesName { get; set; }

        public string SeriesId { get; set; }

        public string SeriesPrimaryImageTag { get; set; }

        public int IndexNumber { get; set; }

        public TvShowSeasonUserData UserData { get; set; }
    }
}
