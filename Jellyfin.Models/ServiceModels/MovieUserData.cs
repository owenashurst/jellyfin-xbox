using System;

namespace Jellyfin.Models.ServiceModels
{
    public class MovieUserData : UserData
    {
        public DateTime LastPlayedDate { get; set; }
    }
}