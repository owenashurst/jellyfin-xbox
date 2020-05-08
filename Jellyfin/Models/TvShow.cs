using System.Collections.ObjectModel;

namespace Jellyfin.Models
{
    /// <summary>
    /// Tv show model representation
    /// </summary>
    public class TvShow : MediaElementBase
    {
        #region Properties

        #region Seasons

        private ObservableCollection<TvShowSeason> _seasons;

        /// <summary>
        /// The property for the seasons of this tv show.
        /// </summary>
        public ObservableCollection<TvShowSeason> Seasons
        {
            get { return _seasons; }
            set
            {
                _seasons = value;
                RaisePropertyChanged(nameof(Seasons));
            }
        }

        #endregion

        #region SelectedSeason

        private TvShowSeason _selectedSeason;

        public TvShowSeason SelectedSeason
        {
            get { return _selectedSeason; }
            set
            {
                _selectedSeason = value;
                RaisePropertyChanged(nameof(SelectedSeason));
            }
        }

        #endregion
        
        #endregion

        #region ctor

        public TvShow()
        {
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