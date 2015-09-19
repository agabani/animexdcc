using System.Threading.Tasks;

namespace AnimeXdcc.Core.Components.Notifications
{
    public interface INotificationListener<in T>
    {
        Task Notify(T obj);
    }
}