using Jellyfin.Core;
using Jellyfin.Models.ServiceModels;

namespace Jellyfin.Models.Adapters
{
    /// <summary>
    /// Adapter to map the JSON tv show season to season model.
    /// </summary>
    public class TvShowSeasonAdapter : IAdapter<TvShowSeasonItem, TvShowSeason>
    {
        #region Additional methods

        public TvShowSeason Convert(TvShowSeasonItem source)
        {
            TvShowSeason t = new TvShowSeason();

            t.Id = source.Id;
            t.Name = source.Name;
            t.ProductionYear = source.ProductionYear.ToString();
            t.IndexNumber = source.IndexNumber;

            if (source.ImageTags != null) { 
                t.ImageId = source.ImageTags.Primary;
            }

            t.UnplayedItemCount = source.UserData.UnplayedItemCount;

            return t;
        }

        #endregion
    }
}