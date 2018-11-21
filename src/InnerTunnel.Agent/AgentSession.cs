using System;
using System.Collections.Generic;
using System.Text;
using InnerTunnel.Common;
using waxbill;
using waxbill.Packets;
using waxbill.Pools;
using waxbill.Sessions;

namespace InnerTunnel.Agent
{
    public class AgentSession : SessionBase
    {
        protected override void OnConnected()
        {
            AgentServer.Instance.SetClient(this);
        }

        protected override void OnDisconnected(CloseReason reason)
        {
            AgentServer.Instance.SetClient(null);
            FromServerManager.Instance.TunnelCloseAll();
        }

        protected override void OnReceived(Packet packet)
        {
            InnerTunnelPacket innerPacket = packet as InnerTunnelPacket;
            if (innerPacket == null)
            {
                return;
            }
            if (innerPacket.Action == 2)
            {
                FromServerManager.Instance.CloseSession(innerPacket.ServicePort, innerPacket.ClientIdentity);
                return;
            }
            byte[] datas = innerPacket.Read();
            FromServerManager.Instance.SendData(innerPacket.ServicePort, innerPacket.ClientIdentity, datas);
        }

        protected override void OnSended(SendingQueue packet, bool result)
        {}
    }
}
