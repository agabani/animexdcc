using System;

namespace AnimeXdcc.Core.Irc.DccMessage
{
    public class DccSendMessage : EventArgs
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}