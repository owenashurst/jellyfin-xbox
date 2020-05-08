using System;

namespace Jellyfin.Models.ServiceModels
{
    public class TvShowItem : Item
    {
        public string[] AirDays { get; set; }
        public string AirTime { get; set; }
        public string[] BackdropImageTags { get; set; }
        public string ParentBackdropItemId { get; set; }
        public string[] ParentBackdropImageTags { get; set; }
       
        public DateTime PremiereDate { get; set; }
        
        public string Status { get; set; }
        public DateTime EndDate { get; set; }

        public UserData UserData { get; set; }
    }

}
