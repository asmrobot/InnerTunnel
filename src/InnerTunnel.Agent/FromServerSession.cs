using System;
using System.Collections.Generic;
using System.Text;
using waxbill;
using waxbill.Packets;
using waxbill.Pools;
using waxbill.Sessions;

namespace InnerTunnel.Agent
{
    public class FromServerSession : SessionBase
    {
        protected override void OnConnected()
        {
            FromServer fsi = this.Monitor as FromServer;
            if (fsi == null)
            {
                return;
            }
            if (!AgentServer.Instance.SendConnect(fsi.ServicePort, this.ConnectionID))
            {
                this.Close(CloseReason.RemoteClose);
            }
        }

        protected override void OnDisconnected(CloseReason reason)
        {
            FromServer fsi = this.Monitor as FromServer;
            if (fsi == null)
            {
                return;
            }
            AgentServer.Instance.SendDisconnect(fsi.ServicePort, this.ConnectionID);
        }

        protected override void OnReceived(Packet packet)
        {
            FromServer fsi = this.Monitor as FromServer;
            if (fsi == null)
            {
                return;
            }
            byte[] datas = packet.Read();
            AgentServer.Instance.SendDatas(fsi.ServicePort, this.ConnectionID, datas);
        }

        protected override void OnSended(SendingQueue packet, bool result)
        { }
    }
}
