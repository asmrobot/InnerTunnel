using System;
using System.Collections.Generic;
using System.Text;
using waxbill;
using waxbill.Packets;
using waxbill.Sessions;

namespace InnerTunnel.Agent.Sessions
{
    public class ToSession : ServerSession
    {
        protected override void OnConnected()
        {
            throw new NotImplementedException();
        }

        protected override void OnDisconnected(CloseReason reason)
        {
            throw new NotImplementedException();
        }

        protected override void OnReceived(Packet packet)
        {
            throw new NotImplementedException();
        }

        protected override void OnSended(PlatformBuf packet, bool result)
        {
            throw new NotImplementedException();
        }
    }
}
