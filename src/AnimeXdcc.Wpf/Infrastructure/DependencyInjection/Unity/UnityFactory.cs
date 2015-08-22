using Microsoft.Practices.Unity;

namespace AnimeXdcc.Wpf.Infrastructure.DependencyInjection.Unity
{
    public class UnityFactory
    {
        public UnityResolver Create()
        {
            var unityContainer = new UnityContainer();
            RegisterTypes(ref unityContainer);
            return new UnityResolver(unityContainer);
        }

        private void RegisterTypes(ref UnityContainer container)
        {
        }
    }
}