namespace Generic.IrcClient.Dcc
{
    public class DccSendMessage
    {
        public string FileName { get; set; }
        public uint FileSize { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}