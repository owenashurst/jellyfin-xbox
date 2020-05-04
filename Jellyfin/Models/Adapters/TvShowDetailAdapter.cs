using System;
using System.Linq;
using Jellyfin.Models.ServiceModels.TvShow;

namespace Jellyfin.Models.Adapters
{
    /// <summary>
    /// Adapter to map the JSON Item to tv show.
    /// </summary>
    public class TvShowDetailAdapter : IAdapter<TvShowDetailsResult, TvShow>
    {
        #region Additional methods

        public TvShow Convert(TvShowDetailsResult source)
        {
            TvShow t = new TvShow();

            t.Id = source.Id;
            t.Name = source.Name;
            t.ImageId = source.ImageTags.Primary;

            if (source.BackdropImageTags != null)
            {
                t.BackdropImageId = source.BackdropImageTags.FirstOrDefault();
            }

            t.CommunityRating = source.CommunityRating.ToString();
            t.Runtime = new TimeSpan(source.RunTimeTicks);
            t.PlaybackPosition = new TimeSpan(source.UserData.PlaybackPositionTicks);
            t.IsPlayed = source.UserData.Played;
            t.Genres = source.Genres;
            t.Description = source.Overview;

            return t;
        }

        #endregion
    }
}