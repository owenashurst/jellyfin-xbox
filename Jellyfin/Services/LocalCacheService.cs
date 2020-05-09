using System;
using System.Threading.Tasks;
using Windows.Storage;
using Jellyfin.Services.Interfaces;
using Newtonsoft.Json;
using Exception = System.Exception;

namespace Jellyfin.Services
{
    public class LocalCacheService : ILocalCacheService
    {
        #region Properties

        private readonly StorageFolder _localCacheFolder = ApplicationData.Current.LocalCacheFolder;

        #endregion

        #region Additional methods

        public async Task Clear()
        {
            await _localCacheFolder.DeleteAsync();
        }

        public async Task Set(string fileName, object value)
        {
            StorageFile sampleFile = await _localCacheFolder.CreateFileAsync(fileName);
            await FileIO.WriteTextAsync(sampleFile, JsonConvert.SerializeObject(value));
        }

        public async Task<T> Get<T>(string fileName, T defaultValue = default(T))
        {
            StorageFile sampleFile = (StorageFile) await _localCacheFolder.TryGetItemAsync(fileName);
            if (sampleFile == null)
            {
                return default(T);
            }

            string fileContent = await FileIO.ReadTextAsync(sampleFile);

            try
            {
                return JsonConvert.DeserializeObject<T>(fileContent);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        #endregion
    }
}