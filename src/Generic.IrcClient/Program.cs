namespace Generic.IrcClient
{
    public class Program
    {
        public void Main()
        {
            var xdccIrcClient = new XdccIrcClient("speech", "speech", "speech", "irc.rizon.net", 6667, "#intel");

            xdccIrcClient.Run();
        }
    }
}