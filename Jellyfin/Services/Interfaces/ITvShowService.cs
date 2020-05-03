using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Models;

namespace Jellyfin.Services.Interfaces
{
    /// <summary>
    /// Interface for the TV service.
    /// </summary>
    public interface ITvShowService
    {
        /// <summary>
        /// Retrieves generic tv show result set.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TvShow>> GetTvShows();

        /// <summary>
        /// Retrieves detailed TV show object.
        /// </summary>
        /// <param name="tvShowId">the ID of the TV show to be retrieved.</param>
        /// <returns></returns>
        Task<TvShow> GetTvShowDetails(string tvShowId);

        /// <summary>
        /// Retrieves tv shows which are related to the provided tv show.
        /// </summary>
        /// <param name="tvShowId">the ID of the tv show for finding similars.</param>
        /// <returns></returns>
        Task<IEnumerable<TvShow>> GetRelatedTvShows(string tvShowId);
    }
}