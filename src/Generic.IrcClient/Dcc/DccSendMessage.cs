﻿namespace Generic.IrcClient.Dcc
{
    public class DccSendMessage
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}