using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Models;

namespace Jellyfin.Services.Interfaces
{
    /// <summary>
    /// Interface for the liking a media element or mark as watched.
    /// </summary>
    public interface IPersonalizeService
    {
        Task MarkAsWatched(string id);

        Task MarkAsUnwatched(string id);

        Task Like(string id);

        Task Dislike(string id);
    }
}