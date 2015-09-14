using System;
using System.Threading.Tasks;

namespace AnimeXdcc.Core.Components.Notifications
{
    public class NotificationListener<T> : INotificationListener<T>
    {
        private readonly Func<T, Task> _executeAsync;

        public NotificationListener(Func<T, Task> executeAsync)
        {
            _executeAsync = executeAsync;
        }

        public Task Notify(T obj)
        {
            return _executeAsync(obj);
        }
    }
}