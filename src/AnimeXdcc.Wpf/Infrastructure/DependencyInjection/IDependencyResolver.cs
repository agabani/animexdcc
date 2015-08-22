using System;

namespace AnimeXdcc.Wpf.Infrastructure.DependencyInjection
{
    public interface IDependencyResolver : IDisposable
    {
        object GetService(Type serviceType);
    }
}