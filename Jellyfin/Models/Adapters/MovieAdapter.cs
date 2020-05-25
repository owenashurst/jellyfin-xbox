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
            
            if (source.UserData != null)
            {
                m.UserData = new MediaUserData
                {
                    Played = source.UserData.Played,
                    IsFavorite = source.UserData.IsFavorite,
                    Key = source.UserData.Key,
                    PlaybackPositionTicks = source.UserData.PlaybackPositionTicks,
                    PlayCount = source.UserData.PlayCount,
                    PlayedPercentage = source.UserData.PlayedPercentage,
                };

                m.PlaybackPosition = new TimeSpan(source.UserData.PlaybackPositionTicks);
                m.IsPlayed = source.UserData.Played;
            }

            return m;
        }
    }
}
