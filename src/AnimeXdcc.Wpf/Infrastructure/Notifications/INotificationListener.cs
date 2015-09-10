namespace AnimeXdcc.Wpf.Infrastructure.Notifications
{
    public interface INotificationListener<in T>
    {
        void Notify(T obj);
    }
}