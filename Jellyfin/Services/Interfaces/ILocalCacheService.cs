using System.Threading.Tasks;

namespace Jellyfin.Services.Interfaces
{
    public interface ILocalCacheService
    {
        Task Clear();

        Task Set(string fileName, object value);

        Task<T> Get<T>(string fileName, T defaultValue = default(T));
    }
}