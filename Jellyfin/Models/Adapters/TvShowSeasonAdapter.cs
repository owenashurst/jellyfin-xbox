using Jellyfin.Core;
using Jellyfin.Models.ServiceModels;

namespace Jellyfin.Models.Adapters
{
    /// <summary>
    /// Adapter to map the JSON tv show season to season model.
    /// </summary>
    public class TvShowSeasonAdapter : ItemMediaElementBaseAdapter, IAdapter<TvShowSeasonItem, TvShowSeason>
    {
        #region Additional methods

        public TvShowSeason Convert(TvShowSeasonItem source)
        {
            TvShowSeason t = new TvShowSeason();
            ConvertBase(source, t);

            t.IndexNumber = source.IndexNumber;
            t.UnplayedItemCount = source.UserData.UnplayedItemCount;

            return t;
        }

        #endregion
    }
}