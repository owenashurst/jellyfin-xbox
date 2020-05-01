using System;
using System.Threading.Tasks;

namespace Jellyfin.Services.Interfaces
{
    /// <summary>
    /// Interface for the reporting playback progress.
    /// </summary>
    public interface IReportProgressService
    {
        /// <summary>
        /// Reports the playback progress back to the server.
        /// </summary>
        /// <returns></returns>
        Task<bool> Report(string id, string playMethod, TimeSpan currentPlaybackTime);
    }
}