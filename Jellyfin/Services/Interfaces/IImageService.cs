using System.Threading.Tasks;
using Jellyfin.Models;

namespace Jellyfin.Services.Interfaces
{
    /// <summary>
    /// Interface for the image service.
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Retrieves an image by its parent id, image id and image result type.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="imageId"></param>
        /// <param name="imageType"></param>
        /// <returns></returns>
        Task<byte[]> GetImage(string id, string imageId, ImageTypeEnum imageType);
    }
}