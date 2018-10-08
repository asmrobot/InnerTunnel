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
        /// <summary>
        /// 用户端与来源服务器的连接ID
        /// </summary>
        public Int64 ConnectionID { get; set; }

        /// <summary>
        /// 要连接服务的端口号
        /// </summary>
        public Int32 ServicePort { get; set; }

        public ServiceClient(Int32 servicePort,Int64 connectionID) : base(new waxbill.Protocols.RealtimeProtocol())
        {
            this.ServicePort = servicePort;
            this.ConnectionID = connectionID;
            this.OnDisconnected += ServiceClient_OnDisconnected;
            this.OnReceive += ServiceClient_OnReceive;
            this.OnConnection += ServiceClient_OnConnection;
        }
        

        private void ServiceClient_OnConnection(TCPClient client, SessionBase session)
        {
            ZTImage.Log.Trace.Info("service is connected");
        }

        private void ServiceClient_OnReceive(TCPClient client, SessionBase session, Packet packet)
        {
            
            byte[] datas = packet.Read();
            AgentClient.Instance.SendData(this.ServicePort,this.ConnectionID, datas);
        }

        private void ServiceClient_OnDisconnected(TCPClient client, SessionBase session, Exception exception)
        {
            AgentClient.Instance.SendDisconnect(this.ServicePort,this.ConnectionID);
            ZTImage.Log.Trace.Info("service is disconnected");
        }
        
    }
}
