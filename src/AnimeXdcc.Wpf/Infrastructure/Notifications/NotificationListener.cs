using System;

namespace AnimeXdcc.Wpf.Infrastructure.Notifications
{
    public class NotificationListener<T> : INotificationListener<T>
    {
        private readonly Action<T> _action;

        public NotificationListener(Action<T> action)
        {
            _action = action;
        }

        public void Notify(T obj)
        {
            _action(obj);
        }
    }
}