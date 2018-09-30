using System;
using System.Collections.Generic;
using System.Text;
using InnerTunnel.Common;
using waxbill;
using waxbill.Packets;
using waxbill.Sessions;
using static InnerTunnel.Agent.ToServer;

namespace InnerTunnel.Agent
{
    public class ToServer: TCPServer<ToSession>
    {
        private ToServer():base(new InnerTunnelProtocol())
        {}

        private ToSession clientSession;

        #region Public Interface

        /// <summary>
        /// 发送连接
        /// </summary>
        /// <param name="connectID"></param>
        public void SendConnect(long connectionID)
        {
            if (clientSession == null)
            {
                return;
            }

            if (clientSession.IsClosed)
            {
                return;
            }

            byte[] send = PacketHelper.Serialize(connectionID, 0, null);
            clientSession.Send(send);

        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="datas"></param>
        public void SendDatas(Int64 connectionID,byte[] datas)
        {
            if (clientSession == null)
            {
                return;
            }

            if (clientSession.IsClosed)
            {
                return;
            }

            byte[] send = PacketHelper.Serialize(connectionID, 1, datas);
            clientSession.Send(send);
        }


        /// <summary>
        /// 发送断开连接
        /// </summary>
        /// <param name="connectID"></param>
        public void SendDisconnect(long connectionID)
        {
            if (clientSession == null)
            {
                return;
            }

            if (clientSession.IsClosed)
            {
                return;
            }

            byte[] send = PacketHelper.Serialize(connectionID, 2, null);
            clientSession.Send(send);
        }
        #endregion

        #region singleton
        private static ToServer instance;
        public static object lockHelper = new object();
        public static ToServer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockHelper)
                    {
                        if (instance == null)
                        {
                            instance = new ToServer();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        public class ToSession : ServerSession
        {
            protected override void OnConnected()
            {
                Console.WriteLine("client connect:" + this.RemoteEndPoint.ToString());
                ToServer.Instance.clientSession = this;
            }

            protected override void OnDisconnected(CloseReason reason)
            {
                Console.WriteLine("client disconnect:" + this.RemoteEndPoint.ToString());
                ToServer.Instance.clientSession = null;
                FromServer.Instance.TunnelClose();
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
                    FromServer.Instance.CloseSession(innerPacket.ClientIdentity);
                    return;
                }
                byte[] datas = innerPacket.Read();
                FromServer.Instance.SendData(innerPacket.ClientIdentity, datas);
            }

            protected override void OnSended(PlatformBuf packet, bool result)
            { }
        }
    }
}
