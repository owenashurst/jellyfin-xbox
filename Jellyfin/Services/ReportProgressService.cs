using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Jellyfin.Core;
using Jellyfin.Extensions;
using Jellyfin.Models.ServiceModels;
using Jellyfin.Services.Interfaces;
using Newtonsoft.Json;

namespace Jellyfin.Services
{
    public class ReportProgressService : ServiceBase, IReportProgressService
    {
        #region Properties

        public string ReportProgressEndpoint
        {
            get => $"{Globals.Instance.Host}/Sessions/Playing/Progress";
        }

        #endregion

        #region Additional methods

        /// <summary>
        /// Reports back the progress to the service.
        /// </summary>
        /// <param name="id">the ID of the media source.</param>
        /// <param name="playMethod">The play method: either DirectStream or Transcode.</param>
        /// <param name="currentPlaybackTime">Indicates the current playback of the watched media.</param>
        /// <returns></returns>
        public async Task<bool> Report(string id, string playMethod, TimeSpan currentPlaybackTime)
        {
            Progress p = new Progress();
            p.AudioStreamIndex = 1;
            p.CanSeek = true; // always true
            p.EventName = "timeupdate";
            p.IsMuted = false; // not applicable for xbox
            p.IsPaused = false; // TODO smurancsik: announce report status even if it's paused
            p.ItemId = id;
            p.MaxStreamingBitrate = 4000000; // TODO smurancsik: variable bitrate
            p.MediaSourceId = id;
            p.PlayMethod = playMethod;
            p.PlaySessionId = string.Empty;
            p.PlaybackStartTimeTicks = DateTime.Now.Ticks;
            p.PlaylistItemId = "playlistItem0";
            p.PositionTicks = currentPlaybackTime.Ticks;
            p.RepeatMode = "RepeatNone";
            p.SubtitleStreamIndex = 1;
            p.VolumeLevel = 100; // not really applicable for xbox
            
            string json = JsonConvert.SerializeObject(p);

            using (HttpClient client = new HttpClient())
            {
                client.AddAuthorizationHeaders();
                client.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage result = await client.PostAsync(ReportProgressEndpoint, content);

                return result.IsSuccessStatusCode;
            }
        }

        #endregion
    }
}