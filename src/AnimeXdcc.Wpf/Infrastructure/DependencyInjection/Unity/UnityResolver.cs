using System;
using Microsoft.Practices.Unity;

namespace AnimeXdcc.Wpf.Infrastructure.DependencyInjection.Unity
{
    public class UnityResolver : IDependencyResolver
    {
        private IUnityContainer _container;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            _container = container;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public T GetSerivce<T>()
        {
            try
            {
                return _container.Resolve<T>();
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        ~UnityResolver()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _container != null)
            {
                _container.Dispose();
                _container = null;
            }
        }
    }
}