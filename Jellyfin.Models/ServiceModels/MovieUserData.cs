using System;

namespace Jellyfin.Models.ServiceModels
{
    public class MovieUserData : Userdata
    {
        public DateTime LastPlayedDate { get; set; }
    }
}