namespace Jellyfin.Models.ServiceModels.Movie
{
    public class JellyfinMovieResult
    {
        public MovieItem[] Items { get; set; }

        public long TotalRecordCount { get; set; }

        public long StartIndex { get; set; }
    }
}
