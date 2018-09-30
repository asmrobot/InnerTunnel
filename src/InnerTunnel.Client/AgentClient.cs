using System;
using System.Collections.Generic;
using System.Text;
using InnerTunnel.Common;
using waxbill;
using waxbill.Packets;
using waxbill.Sessions;

namespace InnerTunnel.Client
{
    public class AgentClient:TCPClient
    {
        private AgentClient():base(new InnerTunnelProtocol())
        {
            this.OnConnection += AgentClient_OnConnection;
            this.OnDisconnected += AgentClient_OnDisconnected;
            this.OnReceive += AgentClient_OnReceive;
        }

        private void AgentClient_OnReceive(TCPClient client, SessionBase session, Packet packet)
        {
            InnerTunnelPacket innerPacket = packet as InnerTunnelPacket;
            if (innerPacket == null)
            {
                return;
            }
            byte[] datas = packet.Read();
            ZTImage.Log.Trace.Info("connection id:" + innerPacket.ClientIdentity.ToString() + ",data:" + System.Text.Encoding.ASCII.GetString(datas));
        }

        private void AgentClient_OnDisconnected(TCPClient client, waxbill.Sessions.SessionBase session, Exception exception)
        {
            
        }

        private void AgentClient_OnConnection(TCPClient client, waxbill.Sessions.SessionBase session)
        {
            
        }


        #region singleton
        private static AgentClient instance;
        public static object lockHelper = new object();
        public static AgentClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockHelper)
                    {
                        if (instance == null)
                        {
                            instance = new AgentClient();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion
    }
}
