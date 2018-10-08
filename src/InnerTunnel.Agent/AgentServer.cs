using System;
using System.Collections.Generic;
using System.Text;
using InnerTunnel.Common;
using waxbill;
using waxbill.Packets;
using waxbill.Sessions;
using static InnerTunnel.Agent.AgentServer;

namespace InnerTunnel.Agent
{
    public class AgentServer: TCPServer<AgentSession>
    {
        private AgentServer():base(new InnerTunnelProtocol())
        {}

        private AgentSession clientSession;

        #region Public Interface

        /// <summary>
        /// 发送连接
        /// </summary>
        /// <param name="connectID"></param>
        public void SendConnect(Int32 servicePort, long connectionID)
        {
            if (clientSession == null)
            {
                return;
            }

            if (clientSession.IsClosed)
            {
                return;
            }



            byte[] send = PacketHelper.Serialize(servicePort,connectionID, 0, null);
            clientSession.Send(send);

        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="datas"></param>
        public void SendDatas(Int32 servicePort,Int64 connectionID,byte[] datas)
        {
            if (clientSession == null)
            {
                return;
            }

            if (clientSession.IsClosed)
            {
                return;
            }

            byte[] send = PacketHelper.Serialize(servicePort, connectionID, 1, datas);
            clientSession.Send(send);

        }


        /// <summary>
        /// 发送断开连接
        /// </summary>
        /// <param name="connectID"></param>
        public void SendDisconnect(Int32 servicePort, long connectionID)
        {
            if (clientSession == null)
            {
                return;
            }

            if (clientSession.IsClosed)
            {
                return;
            }

            byte[] send = PacketHelper.Serialize(servicePort, connectionID, 2, null);
            clientSession.Send(send);
        }
        #endregion

        #region singleton
        private static AgentServer instance;
        public static object lockHelper = new object();
        public static AgentServer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockHelper)
                    {
                        if (instance == null)
                        {
                            instance = new AgentServer();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        public class AgentSession : ServerSession
        {
            protected override void OnConnected()
            {
                Console.WriteLine("client connect:" + this.RemoteEndPoint.ToString());
                AgentServer.Instance.clientSession = this;
            }

            protected override void OnDisconnected(CloseReason reason)
            {
                Console.WriteLine("client disconnect:" + this.RemoteEndPoint.ToString());
                AgentServer.Instance.clientSession = null;
                FromServer.Instance.TunnelCloseAll();
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
                    FromServer.Instance.CloseSession(innerPacket.ServicePort,innerPacket.ClientIdentity);
                    return;
                }
                byte[] datas = innerPacket.Read();
                FromServer.Instance.SendData(innerPacket.ServicePort, innerPacket.ClientIdentity, datas);
            }

            protected override void OnSended(PlatformBuf packet, bool result)
            { }
        }
    }
}
