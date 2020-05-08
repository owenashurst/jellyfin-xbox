using System;

namespace Jellyfin.Models.ServiceModels
{
    public class Item
    {
        public string Name { get; set; }

        public string ServerId { get; set; }

        public string Id { get; set; }

        public string OfficialRating { get; set; }

        public string CommunityRating { get; set; }

        public long RunTimeTicks { get; set; }

        public long ProductionYear { get; set; }

        public bool IsFolder { get; set; }

        public string Type { get; set; }

        public double PrimaryImageAspectRatio { get; set; }

        public Imagetags ImageTags { get; set; }

        public string LocationType { get; set; }

    }
}