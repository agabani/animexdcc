using System;
using System.Collections.Generic;
using AnimeXdcc.Core;
using AnimeXdcc.Core.Components.HumanReadable;
using AnimeXdcc.Core.Components.UserName;
using AnimeXdcc.Core.Logging;
using AnimeXdcc.Core.Logging.Trace;
using AnimeXdcc.Wpf.Download;
using AnimeXdcc.Wpf.General;
using AnimeXdcc.Wpf.Search;
using AnimeXdcc.Wpf.Services;
using AnimeXdcc.Wpf.Services.Search;
using AnimeXdcc.Wpf.Services.Search.Searchable;
using Intel.Haruhichan.ApiClient.Clients;
using Microsoft.Practices.Unity;

namespace AnimeXdcc.Wpf.Infrastructure.DependencyInjection.Unity
{
    public class UnityFactory
    {
        public UnityResolver Create()
        {
            var unityContainer = new UnityContainer();

            RegisterLogger(unityContainer);
            RegisterUtilities(unityContainer);
            RegisterIntel(unityContainer);
            RegisterSearch(unityContainer);
            RegisterDownload(unityContainer);
            RegisterViewModels(unityContainer);

            return new UnityResolver(unityContainer);
        }

        private static void RegisterLogger(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<ILogger, TraceLogger>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(TraceLogger.Level.Debug));
        }

        private void RegisterUtilities(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<IUserNameGenerator, UserNameGenerator>();
            unityContainer.RegisterType<IBytesConvertor, BytesConvertor>();
        }

        public static void RegisterIntel(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<Uri, Uri>("intel", new InjectionConstructor("http://intel.haruhichan.com"));

            unityContainer.RegisterType<IIntelHttpClient, IntelHttpClient>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(unityContainer.Resolve<Uri>("intel"), unityContainer.Resolve<ILogger>()));

            unityContainer.RegisterType<ISearchable, IntelSearchable>("intel",
                new InjectionConstructor(unityContainer.Resolve<IIntelHttpClient>()));
        }

        private static void RegisterSearch(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<ISearchService, SearchService>(new InjectionConstructor(
                new List<ISearchable> {unityContainer.Resolve<ISearchable>("intel")}));
        }

        private void RegisterDownload(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<IAnimeXdccClient, AnimeXdccClient>(
                new InjectionConstructor("irc.rizon.net", 6667, unityContainer.Resolve<IUserNameGenerator>().Create(10)));

            unityContainer.RegisterType<IAnimeXdccService, AnimeXdccService>(
                new InjectionConstructor(unityContainer.Resolve<IAnimeXdccClient>()));
        }

        private static void RegisterViewModels(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<HomeViewModel, HomeViewModel>();

            unityContainer.RegisterType<AboutViewModel, AboutViewModel>();

            unityContainer.RegisterType<SearchEpisodeViewModel, SearchEpisodeViewModel>(
                new InjectionConstructor(unityContainer.Resolve<ISearchService>()));

            unityContainer.RegisterType<DownloadEpisodeViewModel, DownloadEpisodeViewModel>();
        }
    }
}