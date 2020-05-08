using System.Collections.ObjectModel;

namespace Jellyfin.Models
{
    /// <summary>
    /// Tv show model representation
    /// </summary>
    public class TvShow : MediaElementBase
    {
        #region Properties

        public ObservableCollection<Episode> Episodes { get; set; }

        /// <summary>
        /// The property for the seasons of this tv show.
        /// </summary>
        public ObservableCollection<TvShowSeason> Seasons { get; set; }

        #endregion

        #region ctor

        public TvShow()
        {
            Episodes = new ObservableCollection<Episode>();
            Seasons = new ObservableCollection<TvShowSeason>();
        }

        #endregion
        
        #region Overrides

        public override string ToString()
        {
            return $"<{Id}> {Name}";
        }

        #endregion
    }
}