namespace Jellyfin.Models
{
    public class TvShowSeason : MediaElementBase
    {
        #region Properties

        public string ProductionYear { get; set; }

        public int IndexNumber { get; set; }

        public int UnplayedItemCount { get; set; }

        #endregion

        #region ctor

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"<{Id}> {Name}";
        }

        #endregion

    }
}
