namespace Jellyfin.Models
{
    /// <summary>
    /// Tv show model representation
    /// </summary>
    public class TvShow : MediaElementBase
    {
        #region Overrides

        public override string ToString()
        {
            return $"<{Id}> {Name}";
        }

        #endregion
    }
}