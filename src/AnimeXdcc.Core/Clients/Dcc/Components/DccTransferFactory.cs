﻿namespace AnimeXdcc.Core.Clients.Dcc.Components
{
    public class DccTransferFactory : IDccTransferFactory
    {
        public IDccTransfer Create(string hostname, int port)
        {
            return new DccTransfer(hostname, port);
        }
    }
}
