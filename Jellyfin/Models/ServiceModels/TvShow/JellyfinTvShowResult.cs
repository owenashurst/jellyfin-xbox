namespace Jellyfin.Models.ServiceModels.TvShow
{

    public class JellyfinTvShowResult
    {
        public Item[] Items { get; set; }
        public int StartIndex { get; set; }
        public int TotalRecordCount { get; set; }
    }
}
