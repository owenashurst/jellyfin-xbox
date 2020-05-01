using System;

namespace Jellyfin.Models.ServiceModels.Movie
{
    public class Chapter
    {
        public long StartPositionTicks { get; set; }
        public string Name { get; set; }
        public DateTime ImageDateModified { get; set; }
    }
}