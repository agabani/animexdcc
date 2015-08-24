using System;
using AnimeXdcc.Core.Logging;
using AnimeXdcc.Core.Logging.Trace;
using AnimeXdcc.Wpf.Search;
using AnimeXdcc.Wpf.Services;
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
            RegisterIntel(unityContainer);
            RegisterViewModels(unityContainer);

            return new UnityResolver(unityContainer);
        }

        private static void RegisterLogger(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<ILogger, TraceLogger>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(TraceLogger.Level.Debug));
        }

        public static void RegisterIntel(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<Uri, Uri>("intel", new InjectionConstructor("http://intel.haruhichan.com"));

            unityContainer.RegisterType<IIntelHttpClient, IntelHttpClient>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(unityContainer.Resolve<Uri>("intel"), unityContainer.Resolve<ILogger>()));

            unityContainer.RegisterType<IIntelService, IntelService>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(unityContainer.Resolve<IIntelHttpClient>()));
        }

        private static void RegisterViewModels(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<EpisodeSearchViewModel, EpisodeSearchViewModel>(
                new InjectionConstructor(unityContainer.Resolve<IIntelService>()));

            unityContainer.RegisterType<EpisodeSearchResultsViewModel, EpisodeSearchResultsViewModel>(
                new InjectionConstructor(unityContainer.Resolve<IIntelService>()));
        }
    }
}