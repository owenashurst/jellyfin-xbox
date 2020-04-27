using System;

namespace Jellyfin.Models.ServiceReturnModels.Movie
{
    public class Chapter
    {
        public long StartPositionTicks { get; set; }
        public string Name { get; set; }
        public DateTime ImageDateModified { get; set; }
    }
}