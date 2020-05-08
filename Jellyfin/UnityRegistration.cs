using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.Models.Adapters;
using Jellyfin.Models.ServiceModels;
using Jellyfin.Models.ServiceModels.Movie;
using Jellyfin.Models.ServiceModels.TvShow;
using Jellyfin.Services;
using Jellyfin.Services.Interfaces;
using Unity;
using Mediasource = Jellyfin.Models.ServiceModels.PlaybackInformation.Mediasource;

namespace Jellyfin
{
    /// <summary>
    /// Provides access to registered instances used by this application.
    /// </summary>
    public sealed class UnityRegistration
    {
        /// <summary>
        /// Registers type mappings for Unity.
        /// </summary>
        public static void RegisterTypes()
        {
            IUnityContainer container = Globals.Instance.Container;

            RegisterServices(container);
            RegisterAdapters(container);
        }

        public static void RegisterAdapters(IUnityContainer container)
        {
            container.RegisterType<IAdapter<MovieItem, Movie>, MovieAdapter>();
            container.RegisterType<IAdapter<TvShowItem, TvShow>, TvShowAdapter>();
            container.RegisterType<IAdapter<TvShowDetailsResult, TvShow>, TvShowDetailAdapter>();
            container.RegisterType<IAdapter<TvShowSeasonItem, TvShowSeason>, TvShowSeasonAdapter>();
            container.RegisterType<IAdapter<TvShowEpisodeItem, TvShowEpisode>, TvShowEpisodeAdapter>();

            container.RegisterType<
                IAdapter<Mediasource, MediaElementPlaybackSource>,
                MediaElementPlaybackSourceAdapter>();
            container.RegisterType<IAdapter<MovieDetailsResult, Movie>, MovieDetailAdapter>();
        }

        public static void RegisterServices(IUnityContainer container)
        {
            container.RegisterType<IJellyfinNavigationService, JellyfinNavigationService>();

            container.RegisterType<IMovieService, MovieService>();
            container.RegisterType<ITvShowService, TvShowService>();
            container.RegisterType<ILoginService, LoginService>();
            container.RegisterType<IImageService, ImageService>();
            container.RegisterType<ISettingsService, SettingsService>();
            container.RegisterType<IPlaybackInfoService, PlaybackInfoService>();
            container.RegisterType<IReportProgressService, ReportProgressService>();
        }
    }
}
