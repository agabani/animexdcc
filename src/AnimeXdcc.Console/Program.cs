using System.Threading.Tasks;

namespace AnimeXdcc.Console
{
    internal class Program
    {
        public static void Main()
        {
            Task.Run(async () =>
            {
                using (var application = new Application())
                {
                    await application.RunAsync();
                }
            }).Wait();
        }
    }
}