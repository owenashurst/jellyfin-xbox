namespace Jellyfin.Models.ServiceModels.TvShow
{
    public class TvShowEpisodeResult
    {
        public TvShowEpisodeItem[] Items { get; set; }
        public int TotalRecordCount { get; set; }
        public int StartIndex { get; set; }
    }
}