using Jellyfin.Extensions;

namespace Jellyfin.Models
{
    public class TvShowSeason : MediaElementBase
    {
        #region Properties

        public string ProductionYear { get; set; }

        public int IndexNumber { get; set; }

        public int UnplayedItemCount { get; set; }

        #region Episodes

        private ObservableCollectionEx<TvShowEpisode> _tvShowEpisodes;

        public ObservableCollectionEx<TvShowEpisode> TvShowEpisodes
        {
            get { return _tvShowEpisodes; }
            set
            {
                _tvShowEpisodes = value;
                RaisePropertyChanged(nameof(TvShowEpisodes));
            }
        }

        #endregion

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
