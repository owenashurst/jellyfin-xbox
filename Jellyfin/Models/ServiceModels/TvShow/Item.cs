using System;

namespace Jellyfin.Models.ServiceModels.TvShow
{
    public class Item
    {
        public string[] AirDays { get; set; }
        public string AirTime { get; set; }
        public string[] BackdropImageTags { get; set; }
        public float CommunityRating { get; set; }
        public string Id { get; set; }
        public Imagetags ImageTags { get; set; }
        public bool IsFolder { get; set; }
        public string LocationType { get; set; }
        public string Name { get; set; }
        public string OfficialRating { get; set; }
        public DateTime PremiereDate { get; set; }
        public float PrimaryImageAspectRatio { get; set; }
        public int ProductionYear { get; set; }
        public long RunTimeTicks { get; set; }
        public string ServerId { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public Userdata UserData { get; set; }
        public DateTime EndDate { get; set; }
        public int CriticRating { get; set; }
    }

}
