using System;
using System.Collections.Generic;
using System.Text;
using InnerTunnel.Common;
using waxbill;
using waxbill.Packets;
using waxbill.Pools;
using waxbill.Sessions;
using static InnerTunnel.Agent.AgentServer;

namespace InnerTunnel.Agent
{
    public class AgentServer: TCPServer<AgentSession>
    {
        private AgentServer():base(new InnerTunnelProtocol())
        {}

        private AgentSession clientSession;

        public void SetClient(AgentSession session)
        {
            this.clientSession = session;
        }

        #region Public Interface

        /// <summary>
        /// 发送连接
        /// </summary>
        /// <param name="connectID"></param>
        public bool SendConnect(Int32 servicePort, long connectionID)
        {
            if (clientSession == null)
            {
                return false;
            }

            if (clientSession.IsClosed)
            {
                return false;
            }



            byte[] send = PacketHelper.Serialize(servicePort,connectionID, 0, null);
            clientSession.Send(send);
            return true;
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

        
    }
}
