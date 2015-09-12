using System;
using System.Collections.Generic;
using AnimeXdcc.Core;
using AnimeXdcc.Core.Components.Files;
using AnimeXdcc.Core.Components.HumanReadable;
using AnimeXdcc.Core.Components.UserName;
using AnimeXdcc.Core.Dcc.Components;
using AnimeXdcc.Core.Irc.Clients;
using AnimeXdcc.Core.Logging;
using AnimeXdcc.Core.Logging.Trace;
using AnimeXdcc.Wpf.Download;
using AnimeXdcc.Wpf.General;
using AnimeXdcc.Wpf.Search;
using AnimeXdcc.Wpf.Services;
using AnimeXdcc.Wpf.Services.Download;
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
            unityContainer.RegisterType<IDccClientFactory, DccClientFactory>(
                new InjectionConstructor(1000)); // needs to be completed

            unityContainer.RegisterType<IXdccIrcClient, XdccIrcClient>(
                new InjectionConstructor("irc.rizon.net", 6667, unityContainer.Resolve<IUserNameGenerator>().Create(10)));

            unityContainer.RegisterType<IStreamProvider, StreamProvider>(
                new InjectionConstructor());

            unityContainer.RegisterType<IDownloadClient, DownloadClient>(
                new InjectionConstructor(unityContainer.Resolve<IXdccIrcClient>(), unityContainer.Resolve<IDccClientFactory>()));

            unityContainer.RegisterType<IDownloadService, DownloadService>(
                new InjectionConstructor(unityContainer.Resolve<IDownloadClient>(), unityContainer.Resolve<IStreamProvider>()));

            unityContainer.RegisterType<IDownloadQueueService, DownloadQueueService>(
                new InjectionConstructor(unityContainer.Resolve<IDownloadService>()));

            
        }

        private static void RegisterViewModels(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<HomeViewModel, HomeViewModel>();

            unityContainer.RegisterType<AboutViewModel, AboutViewModel>();

            unityContainer.RegisterType<SearchEpisodeViewModel, SearchEpisodeViewModel>(
                new InjectionConstructor(unityContainer.Resolve<ISearchService>()));

            unityContainer.RegisterType<DownloadEpisodeViewModel, DownloadEpisodeViewModel>(
                new InjectionConstructor(unityContainer.Resolve<IDownloadService>()));

            unityContainer.RegisterType<DownloadQueueViewModel, DownloadQueueViewModel>(
                new InjectionConstructor(unityContainer.Resolve<IDownloadQueueService>()));
        }
    }
}