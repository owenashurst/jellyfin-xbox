using Newtonsoft.Json;

namespace Jellyfin.Models.ServiceReturnModels.Movie
{
    public class JellyfinMovieResult
    {
        public Item[] Items { get; set; }

        public long TotalRecordCount { get; set; }

        public long StartIndex { get; set; }
    }
}
