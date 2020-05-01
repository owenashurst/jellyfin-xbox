using Jellyfin.Core;
using Jellyfin.Models;
using Jellyfin.Models.Adapters;
using Jellyfin.Models.ServiceModels.Movie;
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
            RegisterServices();
            RegisterAdapters();
        }

        public static void RegisterAdapters()
        {
            IUnityContainer container = Globals.Instance.Container;

            container.RegisterType<IAdapter<Item, Movie>, MovieAdapter>();
            container.RegisterType<
                IAdapter<Mediasource, MediaElementPlaybackSource>,
                MediaElementPlaybackSourceAdapter>();
            container.RegisterType<IAdapter<MovieDetailsResult, MovieDetail>, MovieDetailAdapter>();
        }

        public static void RegisterServices()
        {
            IUnityContainer container = Globals.Instance.Container;

            container.RegisterType<IJellyfinNavigationService, JellyfinNavigationService>();

            container.RegisterType<IMovieService, MovieService>();
            container.RegisterType<ILoginService, LoginService>();
            container.RegisterType<IImageService, ImageService>();
            container.RegisterType<ISettingsService, SettingsService>();
            container.RegisterType<IPlaybackInfoService, PlaybackInfoService>();
        }
    }
}
