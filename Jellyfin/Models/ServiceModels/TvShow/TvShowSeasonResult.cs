namespace Jellyfin.Models.ServiceModels.TvShow
{

    public class TvShowSeasonResult
    {
        public TvShowSeasonItem[] Items { get; set; }
        public int TotalRecordCount { get; set; }
        public int StartIndex { get; set; }
    }
}
