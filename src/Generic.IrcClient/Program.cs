namespace Generic.IrcClient
{
    public class Program
    {
        public void Main()
        {
            var xdccIrcClient = new XdccIrcClient();

            xdccIrcClient.Run();
        }
    }
}