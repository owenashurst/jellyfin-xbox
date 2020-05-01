using System;

namespace Jellyfin.Models.ServiceModels.Movie
{
    public class Item
    {
        public string Name { get; set; }
        
        public string ServerId { get; set; }
        
        public string Id { get; set; }
        
        public bool HasSubtitles { get; set; }
        
        public string Container { get; set; }
        
        public DateTimeOffset PremiereDate { get; set; }
        
        public long? CriticRating { get; set; }
        
        public string OfficialRating { get; set; }
        
        public double CommunityRating { get; set; }
        
        public long RunTimeTicks { get; set; }
        
        public long ProductionYear { get; set; }
        
        public bool IsFolder { get; set; }
        
        public string Type { get; set; }
        
        public Userdata UserData { get; set; }
        
        public double PrimaryImageAspectRatio { get; set; }
        
        public string VideoType { get; set; }
        
        public Imagetags ImageTags { get; set; }
        
        public string[] BackdropImageTags { get; set; }
        
        public string LocationType { get; set; }

        public string[] Genres { get; set; }
        
        public string MediaType { get; set; }
    }
}