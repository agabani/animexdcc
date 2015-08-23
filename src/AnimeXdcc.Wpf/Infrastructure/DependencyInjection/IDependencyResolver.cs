using System;

namespace AnimeXdcc.Wpf.Infrastructure.DependencyInjection
{
    public interface IDependencyResolver : IDisposable
    {
        T GetSerivce<T>();
    }
}