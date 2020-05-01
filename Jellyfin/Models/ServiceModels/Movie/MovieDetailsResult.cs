using System;

namespace Jellyfin.Models.ServiceModels.Movie
{
    public class MovieDetailsResult
    {
        public string Name { get; set; }
        public string OriginalTitle { get; set; }
        public string ServerId { get; set; }
        public string Id { get; set; }
        public string Etag { get; set; }
        public DateTime DateCreated { get; set; }
        public bool CanDelete { get; set; }
        public bool CanDownload { get; set; }
        public bool HasSubtitles { get; set; }
        public string Container { get; set; }
        public string SortName { get; set; }
        public DateTime PremiereDate { get; set; }
        public Externalurl[] ExternalUrls { get; set; }
        public Mediasource[] MediaSources { get; set; }
        public int CriticRating { get; set; }
        public string[] ProductionLocations { get; set; }
        public string Path { get; set; }
        public bool EnableMediaSourceDisplay { get; set; }
        public string OfficialRating { get; set; }
        public string Overview { get; set; }
        public string[] Taglines { get; set; }
        public string[] Genres { get; set; }
        public float CommunityRating { get; set; }
        public long RunTimeTicks { get; set; }
        public string PlayAccess { get; set; }
        public int ProductionYear { get; set; }
        public Remotetrailer[] RemoteTrailers { get; set; }
        public Providerids ProviderIds { get; set; }
        public bool IsHD { get; set; }
        public bool IsFolder { get; set; }
        public string ParentId { get; set; }
        public string Type { get; set; }
        public Person[] People { get; set; }
        public Studio[] Studios { get; set; }
        public Genreitem[] GenreItems { get; set; }
        public int LocalTrailerCount { get; set; }
        public Userdata UserData { get; set; }
        public int SpecialFeatureCount { get; set; }
        public string DisplayPreferencesId { get; set; }
        public object[] Tags { get; set; }
        public float PrimaryImageAspectRatio { get; set; }
        public Mediastream1[] MediaStreams { get; set; }
        public string VideoType { get; set; }
        public Imagetags ImageTags { get; set; }
        public string[] BackdropImageTags { get; set; }
        public object[] ScreenshotImageTags { get; set; }
        public Chapter[] Chapters { get; set; }
        public string LocationType { get; set; }
        public string MediaType { get; set; }
        public object[] LockedFields { get; set; }
        public bool LockData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
