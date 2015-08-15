using System.Threading.Tasks;

namespace AnimeXdcc.Console
{
    internal class Program
    {
        public static void Main()
        {
            Task.Run(async () => await new Application().RunAsync()).Wait();
        }
    }
}