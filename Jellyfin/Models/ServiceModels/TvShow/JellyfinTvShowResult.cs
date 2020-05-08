namespace Jellyfin.Models.ServiceModels.TvShow
{

    public class JellyfinTvShowResult
    {
        public TvShowItem[] Items { get; set; }
        public int StartIndex { get; set; }
        public int TotalRecordCount { get; set; }
    }
}
