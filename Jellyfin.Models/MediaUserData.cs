using Jellyfin.Core.Models;

namespace Jellyfin.Models
{
    public class MediaUserData : ModelBase
    {
        #region PlayedPercentage

        private float _playedPercentage;

        public float PlayedPercentage
        {
            get { return _playedPercentage; }
            set
            {
                if (_playedPercentage != value)
                {
                    _playedPercentage = value;
                    RaisePropertyChanged(nameof(PlayedPercentage));
                }
            }
        }

        #endregion

        #region PlaybackPositionTicks

        private long _playbackPositionTicks;

        public long PlaybackPositionTicks
        {
            get { return _playbackPositionTicks; }
            set
            {
                if (_playbackPositionTicks != value)
                {
                    _playbackPositionTicks = value;
                    RaisePropertyChanged(nameof(PlaybackPositionTicks));
                }
            }
        }

        #endregion

        #region PlayCount

        private int _playCount;

        public int PlayCount
        {
            get { return _playCount; }
            set
            {
                if (_playCount != value)
                {
                    _playCount = value;
                    RaisePropertyChanged(nameof(PlayCount));
                }
            }
        }

        #endregion

        #region IsFavorite

        private bool _isFavorite;

        public bool IsFavorite
        {
            get { return _isFavorite; }
            set
            {
                if (_isFavorite != value)
                {
                    _isFavorite = value;
                    RaisePropertyChanged(nameof(IsFavorite));
                }
            }
        }

        #endregion

        #region Played

        private bool _played;

        public bool Played
        {
            get { return _played; }
            set
            {
                if (_played != value)
                {
                    _played = value;
                    RaisePropertyChanged(nameof(Played));
                }
            }
        }

        #endregion

        #region Key

        private string _key;

        public string Key
        {
            get { return _key; }
            set
            {
                if (_key != value)
                {
                    _key = value;
                    RaisePropertyChanged(nameof(Key));
                }
            }
        }

        #endregion
    }
}