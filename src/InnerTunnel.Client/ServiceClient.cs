using InnerTunnel.Common;
using System;
using System.Collections.Generic;
using System.Text;
using waxbill;
using waxbill.Packets;
using waxbill.Sessions;

namespace InnerTunnel.Client
{
    public class ServiceClient:TCPClient
    {
        public Int64 ConnectionID { get; set; }

        public ServiceClient(Int64 connectionID) : base(new InnerTunnelProtocol())
        {
            this.ConnectionID = connectionID;
            this.OnDisconnected += AgentClient_OnDisconnected;
            this.OnReceive += AgentClient_OnReceive;
            this.OnConnection += ServiceClient_OnConnection;
        }

        private void ServiceClient_OnConnection(TCPClient client, SessionBase session)
        {
            ZTImage.Log.Trace.Info("service is connected");
        }

        private void AgentClient_OnReceive(TCPClient client, SessionBase session, Packet packet)
        {
            byte[] datas = packet.Read();
            AgentClient.Instance.SendData(this.ConnectionID, datas);
            ZTImage.Log.Trace.Info("service is receive");
        }

        private void AgentClient_OnDisconnected(TCPClient client, SessionBase session, Exception exception)
        {
            AgentClient.Instance.SendDisconnect(this.ConnectionID);
            ZTImage.Log.Trace.Info("service is disconnected");
        }
        
    }
}
