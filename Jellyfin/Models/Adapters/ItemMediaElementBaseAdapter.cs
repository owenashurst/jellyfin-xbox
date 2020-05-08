using System;
using Jellyfin.Models.ServiceModels;

namespace Jellyfin.Models.Adapters
{
    public class ItemMediaElementBaseAdapter
    {
        public void ConvertBase(Item source, MediaElementBase target)
        {
            target.Name = source.Name;
            target.Id = source.Id;
            target.OfficialRating = source.OfficialRating;
            target.CommunityRating = source.CommunityRating;
            target.Runtime = TimeSpan.FromTicks(source.RunTimeTicks);
            target.Year = source.ProductionYear;

            if (source.ImageTags != null)
            {
                target.ImageId = source.ImageTags.Primary;
            }
        }
    }
}
