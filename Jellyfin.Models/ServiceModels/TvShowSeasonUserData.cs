using System;

namespace Jellyfin.Models.ServiceModels
{
    public class TvShowSeasonUserData : Userdata
    {
        public int UnplayedItemCount { get; set; }
    }
}