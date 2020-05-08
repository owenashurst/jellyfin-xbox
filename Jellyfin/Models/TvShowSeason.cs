using Jellyfin.Extensions;

namespace Jellyfin.Models
{
    public class TvShowSeason : MediaElementBase
    {
        #region Properties

        public TvShow TvShow { get; set; }

        public override string SecondLine
        {
            get => Year;
        }

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

        public TvShowSeason()
        {
            TvShowEpisodes = new ObservableCollectionEx<TvShowEpisode>();
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
