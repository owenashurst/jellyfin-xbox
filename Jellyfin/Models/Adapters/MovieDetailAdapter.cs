using System;
using System.Linq;
using Jellyfin.Core;
using Jellyfin.Models.ServiceModels.Movie;

namespace Jellyfin.Models.Adapters
{
    /// <summary>
    /// Adapter to map the JSON Movie Detail to Movie detail.
    /// </summary>
    public class MovieDetailAdapter : IAdapter<MovieDetailsResult, Movie>
    {
        public Movie Convert(MovieDetailsResult source)
        {
            Movie m = new Movie();

            m.Id = source.Id;
            m.Name = source.Name;
            m.DateCreated = source.DateCreated;
            m.Year = source.ProductionYear.ToString();
            m.ImageId = source.ImageTags.Primary;
            
            m.HasSubtitles = source.HasSubtitles;
            m.CommunityRating = source.CommunityRating.ToString();
            m.OfficialRating = source.OfficialRating;
            m.Runtime = new TimeSpan(source.RunTimeTicks);

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

            m.Genres = source.Genres;
            m.Description = source.Overview;
            m.VideoType = source.MediaStreams.FirstOrDefault(q => q.Type.ToLower() == "video")?.DisplayTitle;
            m.AudioType = source.MediaStreams.FirstOrDefault(q => q.Type.ToLower() == "audio")?.DisplayTitle;

            return m;
        }
    }
}