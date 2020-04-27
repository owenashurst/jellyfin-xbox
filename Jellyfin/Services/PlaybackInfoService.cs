using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Jellyfin.Core;
using Jellyfin.Extensions;
using Jellyfin.Models;
using Jellyfin.Models.Adapters;
using Jellyfin.Models.ServiceReturnModels.PlaybackInformation;
using Jellyfin.Services.Interfaces;
using Newtonsoft.Json;

namespace Jellyfin.Services
{
    public class PlaybackInfoService : ServiceBase, IPlaybackInfoService
    {
        #region Properties

        public string GetPlaybackInfoEndpoint
        {
            get => $"{Globals.Instance.Host}/Items/{{0}}/PlaybackInfo?UserId={Globals.Instance.User.Id}&StartTimeTicks=0&IsPlayback=false&AutoOpenLiveStream=false&MaxStreamingBitrate=4000000";
        }

        #endregion

        /// <summary>
        /// Adapter from media source (coming from the API) to MediaElementPlaybackSource.
        /// </summary>
        private readonly IAdapter<Mediasource, MediaElementPlaybackSource> _mediaElementPlaybackSourceAdapter;

        #region ctor

        public PlaybackInfoService(IAdapter<Mediasource, MediaElementPlaybackSource> mediaElementPlaybackSourceAdapter)
        {
            _mediaElementPlaybackSourceAdapter = mediaElementPlaybackSourceAdapter ??
                throw new ArgumentNullException(nameof(mediaElementPlaybackSourceAdapter));
        }

        #endregion


        #region Additional methods

        /// <summary>
        /// Retrieves playback information based on the media element ID.
        /// </summary>
        /// <param name="id">The media element ID to be played back.</param>
        /// <returns></returns>
        public async Task<IEnumerable<MediaElementPlaybackSource>> GetPlaybackInformation(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            List<MediaElementPlaybackSource> mediaElementPlaybackSources  = new List<MediaElementPlaybackSource>();
            
            string url = string.Format(GetPlaybackInfoEndpoint, id);

            using (HttpClient cli = new HttpClient())
            {
                cli.AddAuthorizationHeaders();

                HttpResponseMessage result = await cli.GetAsync(url);

                if (!result.IsSuccessStatusCode)
                {
                    return new List<MediaElementPlaybackSource>();
                }

                string jsonResult = await result.Content.ReadAsStringAsync();

                PlaybackInformationResult resultSet = JsonConvert.DeserializeObject<PlaybackInformationResult>(jsonResult);

                foreach (Mediasource mediaSource in resultSet.MediaSources)
                {
                    MediaElementPlaybackSource playbackSource = _mediaElementPlaybackSourceAdapter.Convert(mediaSource);
                    mediaElementPlaybackSources.Add(playbackSource);
                }

                return mediaElementPlaybackSources;
            }
        }

        #endregion
    }
}