using System;
using System.Net.Http;
using System.Threading.Tasks;
using Jellyfin.Extensions;
using Jellyfin.Logging;
using Jellyfin.Services.Interfaces;

namespace Jellyfin.Services
{
    public class PersonalizeService : ServiceBase, IPersonalizeService
    {
        #region Properties

        /// <summary>
        /// Indicates whether the media element was liked or not. To set, send with POST, to delete send with DELETE verb.
        /// </summary>
        public string ToggleFavoriteItemEndpoint
        {
            get => $"{Globals.Instance.Host}/Users/{Globals.Instance.User.Id}/FavoriteItems/{{0}}";
        }

        public string MarkItemAsWatchedEndpoint
        {
            //?DatePlayed=20200525145750
            get => $"{Globals.Instance.Host}/Users/{Globals.Instance.User.Id}/PlayedItems/{{0}}?DatePlayed={{1}}";
        }

        public string MarkItemAsUnwatchedEndpoint
        {
            get => $"{Globals.Instance.Host}/Users/{Globals.Instance.User.Id}/PlayedItems/{{0}}";
        }

        /// <summary>
        /// Reference for the log manager.
        /// </summary>
        private readonly ILogManager _logManager;

        #endregion

        #region ctor

        public PersonalizeService(ILogManager logManager)
        {
            _logManager = logManager ??
                          throw new ArgumentNullException(nameof(logManager));
        }
        
        #endregion

        #region Additional methods

        [LogMethod]
        public async Task MarkAsWatched(string id)
        {
            try
            {
                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    await cli.PostAsync(
                        string.Format(MarkItemAsWatchedEndpoint, id, DateTime.Now.ToString("yyyyMMddHHmmss")),
                        new StringContent(string.Empty));
                }
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, $"An error occurred while marking watched media element with ID {id}.");
            }
        }

        [LogMethod]
        public async Task MarkAsUnwatched(string id)
        {
            try
            {
                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    await cli.DeleteAsync(
                        string.Format(MarkItemAsUnwatchedEndpoint, id));
                }
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, $"An error occurred while marking unwatched media element with ID {id}.");
            }
        }

        [LogMethod]
        public async Task Like(string id)
        {
            try
            {
                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    await cli.PostAsync(string.Format(ToggleFavoriteItemEndpoint, id), new StringContent(string.Empty));
                }
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, $"An error occurred while liking media element with ID {id}.");
            }
        }

        [LogMethod]
        public async Task Dislike(string id)
        {
            try
            {
                using (HttpClient cli = new HttpClient())
                {
                    cli.AddAuthorizationHeaders();

                    await cli.DeleteAsync(string.Format(ToggleFavoriteItemEndpoint, id));
                }
            }
            catch (Exception xc)
            {
                _logManager.LogError(xc, $"An error occurred while disliking media element with ID {id}.");
            }
        }

        #endregion
    }
}