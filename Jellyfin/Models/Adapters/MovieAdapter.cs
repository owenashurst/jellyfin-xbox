using System;
using System.Linq;
using Jellyfin.Core;
using Jellyfin.Models.ServiceModels;

namespace Jellyfin.Models.Adapters
{
    /// <summary>
    /// Adapter to map the JSON Item to Movie.
    /// </summary>
    public class MovieAdapter : ItemMediaElementBaseAdapter, IAdapter<MovieItem, Movie>
    {
        public Movie Convert(MovieItem source)
        {
            Movie m = new Movie();

            ConvertBase(source, m);

            if (source.BackdropImageTags != null)
            {
                m.BackdropImageId = source.BackdropImageTags.FirstOrDefault();
            }

            m.HasSubtitles = source.HasSubtitles;
            m.Genres = source.Genres;
            m.PremiereDate = source.PremiereDate;
            m.PlaybackPosition = new TimeSpan(source.UserData.PlaybackPositionTicks);
            m.IsPlayed = source.UserData.Played;

            return m;
        }
    }
}
