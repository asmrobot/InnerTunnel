using System;
using System.Collections.Generic;
using System.Text;
using InnerTunnel.Client.Models;
using InnerTunnel.Common;
using waxbill;
using waxbill.Packets;
using waxbill.Sessions;
using ZTImage.Configuration;

namespace InnerTunnel.Client
{
    public class AgentClient:TCPClient
    {

        private AgentClient():base(new InnerTunnelProtocol())
        {
            config = ConfigHelper.GetInstance<ClientConfigInfo>();
            this.OnDisconnected += AgentClient_OnDisconnected;
            this.OnReceive += AgentClient_OnReceive;

        }

        private Dictionary<Int64, ServiceClient> serviceSessions = new Dictionary<long, ServiceClient>();//连接服务的集合
        private ClientConfigInfo config;


        private void AgentClient_OnReceive(TCPClient client, SessionBase session, Packet packet)
        {
            InnerTunnelPacket innerPacket = packet as InnerTunnelPacket;
            if (innerPacket == null)
            {
                return;
            }
            ServiceClient c;
            if (innerPacket.Action == 0)
            {
                //连接
                c = new ServiceClient(innerPacket.ClientIdentity);
                c.Connection("127.0.0.1",config.ServicePort);
                if (serviceSessions.ContainsKey(innerPacket.ClientIdentity))
                {
                    serviceSessions[innerPacket.ClientIdentity] = c;
                }
                else
                {
                    serviceSessions.Add(innerPacket.ClientIdentity, c);
                }
                return;
            }

            if (!serviceSessions.ContainsKey(innerPacket.ClientIdentity))
            {
                return;
            }
            c = serviceSessions[innerPacket.ClientIdentity];

            if (innerPacket.Action == 2)
            {
                //断开连接                
                c.Disconnect();
                return;
            }
            //转发数据
            byte[] datas = packet.Read();
            c.Send(datas);
            //ZTImage.Log.Trace.Info("connection id:" + innerPacket.ClientIdentity.ToString() + ",data:" + System.Text.Encoding.ASCII.GetString(datas));
        }


        /// <summary>
        /// 关闭所有连接
        /// </summary>
        /// <param name="client"></param>
        /// <param name="session"></param>
        /// <param name="exception"></param>
        private void AgentClient_OnDisconnected(TCPClient client, waxbill.Sessions.SessionBase session, Exception exception)
        {
            //通道关闭后关闭所有连接
            foreach (var item in serviceSessions)
            {
                item.Value.Disconnect();
            }
            serviceSessions.Clear();
        }

        #region Public Interface
        /// <summary>
        /// 向代理端发送数据
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="datas"></param>
        public void SendData(Int64 connectionID, byte[] datas)
        {
            byte[] send = PacketHelper.Serialize(connectionID, 1, datas);
            this.Send(send);
        }

        /// <summary>
        /// 告知代理端服务已断开连接
        /// </summary>
        /// <param name="connectionID"></param>
        public void SendDisconnect(Int64 connectionID)
        {
            byte[] send = PacketHelper.Serialize(connectionID, 2, null);
            this.Send(send);
        }
        #endregion
        

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
