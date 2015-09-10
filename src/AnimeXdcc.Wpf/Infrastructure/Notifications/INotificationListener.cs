using System.Threading.Tasks;

namespace AnimeXdcc.Wpf.Infrastructure.Notifications
{
    public interface INotificationListener<in T>
    {
        Task Notify(T obj);
    }
}