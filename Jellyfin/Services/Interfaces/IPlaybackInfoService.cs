using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Models;

namespace Jellyfin.Services.Interfaces
{
    /// <summary>
    /// Interface for the retrieving playback information.
    /// </summary>
    public interface IPlaybackInfoService
    {
        Task<IEnumerable<MediaElementPlaybackSource>> GetPlaybackInformation(string id);
    }
}