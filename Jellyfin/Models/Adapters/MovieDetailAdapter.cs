using System;
using System.Linq;
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
            m.Year = source.ProductionYear.ToString();
            m.ImageId = source.ImageTags.Primary;
            
            m.HasSubtitles = source.HasSubtitles;
            m.CommunityRating = source.CommunityRating.ToString();
            m.OfficialRating = source.OfficialRating;
            m.Runtime = new TimeSpan(source.RunTimeTicks);
            m.PlaybackPosition = new TimeSpan(source.UserData.PlaybackPositionTicks);
            m.IsPlayed = source.UserData.Played;


            m.Genres = source.Genres;
            m.Description = source.Overview;
            m.VideoType = source.MediaStreams.FirstOrDefault(q => q.Type.ToLower() == "video")?.DisplayTitle;
            m.AudioType = source.MediaStreams.FirstOrDefault(q => q.Type.ToLower() == "audio")?.DisplayTitle;

            return m;
        }
    }
}