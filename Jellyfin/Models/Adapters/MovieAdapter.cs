using System;
using System.Linq;
using Jellyfin.Core;
using Jellyfin.Models.ServiceModels;

namespace Jellyfin.Models.Adapters
{
    /// <summary>
    /// Adapter to map the JSON Item to Movie.
    /// </summary>
    public class MovieAdapter : IAdapter<MovieItem, Movie>
    {
        #region Additional methods

        public Movie Convert(MovieItem source)
        {
            Movie m = new Movie();

            m.Id = source.Id;
            m.Name = source.Name;
            m.Year = source.ProductionYear.ToString();
            m.ImageId = source.ImageTags.Primary;

            if (source.BackdropImageTags != null)
            {
                m.BackdropImageId = source.BackdropImageTags.FirstOrDefault();
            }

            m.HasSubtitles = source.HasSubtitles;
            m.Genres = source.Genres;
            m.PremiereDate = source.PremiereDate;
            m.CommunityRating = source.CommunityRating.ToString();
            m.Runtime = new TimeSpan(source.RunTimeTicks);
            m.PlaybackPosition = new TimeSpan(source.UserData.PlaybackPositionTicks);
            m.IsPlayed = source.UserData.Played;


            return m;
        }

        #endregion
    }
}
