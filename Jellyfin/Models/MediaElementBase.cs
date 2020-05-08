using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Jellyfin.Core.Models;

namespace Jellyfin.Models
{
    public abstract class MediaElementBase : ModelBase
    {
        #region Properties

        /// <summary>
        /// The ID of the movie.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Image ID of the movie.
        /// </summary>
        public string ImageId { get; set; }

        #region ImageData

        private byte[] _imageData;

        /// <summary>
        /// The byte array of the downloaded data of the image.
        /// </summary>
        public byte[] ImageData
        {
            get { return _imageData; }
            set
            {
                _imageData = value;
                Globals.Instance.UIDispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>
                    {
                        RaisePropertyChanged(nameof(ImageData));
                    });
            }
        }

        #endregion

        public abstract string SecondLine { get; }

        /// <summary>
        /// The production year of the media element.
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// The Image ID of the backdrop of the movie.
        /// </summary>
        public string BackdropImageId { get; set; }

        #region BackdropImageData

        private byte[] _backdropImageData;

        /// <summary>
        /// The byte array of the downloaded data of the backdrop image.
        /// </summary>
        public byte[] BackdropImageData
        {
            get { return _backdropImageData; }
            set
            {
                _backdropImageData = value;
                
                #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Globals.Instance.UIDispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>
                    {
                        RaisePropertyChanged(nameof(BackdropImageData));
                    });
                #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        #endregion

        /// <summary>
        /// The title of the media element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The genres of the media element.
        /// </summary>
        public string[] Genres { get; set; }

        public string FormattedGenres
        {
            get
            {
                if (Genres == null)
                {
                    return string.Empty;
                }

                return string.Join(", ", Genres);
            }
        }

        /// <summary>
        /// The community rating of the movie.
        /// See field CommunityRating.
        /// </summary>
        public string CommunityRating { get; set; }

        /// <summary>
        /// The official rating (R, PG13, ...)
        /// </summary>
        public string OfficialRating { get; set; }

        /// <summary>
        /// The length of the movie.
        /// </summary>
        public TimeSpan Runtime { get; set; }

        public string FormattedRuntime
        {
            get
            {
                if (Runtime == TimeSpan.Zero)
                {
                    return string.Empty;
                }

                if (Runtime.TotalHours > 1)
                {
                    return Runtime.Hours + " hr " + Runtime.Minutes + " min";
                }

                return Runtime.Minutes + " min";
            }
        }

        public TimeSpan PlaybackPosition { get; set; }

        public TimeSpan PlaybackRemaining
        {
            get { return Runtime - PlaybackPosition; }
        }

        public string FormattedPlaybackRemaining
        {
            get
            {
                if (PlaybackRemaining == TimeSpan.Zero)
                {
                    return string.Empty;
                }

                if (PlaybackRemaining == Runtime)
                {
                    return " • Unplayed";
                }

                return $" • {PlaybackRemaining.Hours} hr {PlaybackRemaining.Minutes} min remaining";
            }
        }

        public bool IsPlayed { get; set; }

        /// <summary>
        /// The selected video type to playback.
        /// See MediaStreams[TYPE=VIDEO].DisplayTitle
        /// </summary>
        public string VideoType { get; set; }

        /// <summary>
        /// The selected audio type to playback.
        /// See MediaStreams[TYPE=AUDIO].DisplayTitle
        /// </summary>
        public string AudioType { get; set; }
        
        /// <summary>
        /// The "overview" of the movie
        /// See field Overview
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The playback information of the media element.
        /// Comes from PlaybackInfoService.
        /// </summary>
        public IEnumerable<MediaElementPlaybackSource> PlaybackInformation { get; set; }

        #endregion
    }
}