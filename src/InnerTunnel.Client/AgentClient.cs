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
            this.OnReceived += AgentClient_OnReceive;

        }

        private Dictionary<string, ServiceClient> serviceSessions = new Dictionary<string, ServiceClient>();//连接服务的集合
        private ClientConfigInfo config;


        private void AgentClient_OnReceive(TCPClient client, SessionBase session, Packet packet)
        {
            InnerTunnelPacket innerPacket = packet as InnerTunnelPacket;
            if (innerPacket == null)
            {
                return;
            }
            ServiceClient serviceClient;
            string key = innerPacket.ClientIdentity.ToString() + "|" + innerPacket.ServicePort.ToString();
            if (innerPacket.Action == 0)
            {
                //连接
                //获取目标端口号
                serviceClient = new ServiceClient(innerPacket.ServicePort,innerPacket.ClientIdentity);
                serviceClient.Connect(config.ServiceIP,innerPacket.ServicePort);
                if (serviceSessions.ContainsKey(key))
                {
                    serviceSessions[key] = serviceClient;
                }
                else
                {
                    serviceSessions.Add(key, serviceClient);
                }
                return;
            }

            if (!serviceSessions.ContainsKey(key))
            {
                return;
            }
            serviceClient = serviceSessions[key];

            if (innerPacket.Action == 2)
            {
                //断开连接      
                serviceSessions.Remove(key);
                serviceClient.Disconnect();
                return;
            }
            //转发数据
            byte[] datas = packet.Read();
            
            serviceClient.Send(datas);
        }


        /// <summary>
        /// 关闭所有连接
        /// </summary>
        /// <param name="client"></param>
        /// <param name="session"></param>
        /// <param name="exception"></param>
        private void AgentClient_OnDisconnected(TCPClient client, waxbill.Sessions.SessionBase session, CloseReason exception)
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
        public void SendData(Int32 servicePort,Int64 connectionID, byte[] datas)
        {
            byte[] send = PacketHelper.Serialize(servicePort,connectionID, 1, datas);
            this.Send(send);
        }

        /// <summary>
        /// 告知代理端服务已断开连接
        /// </summary>
        /// <param name="connectionID"></param>
        public void SendDisconnect(Int32 servicePort,Int64 connectionID)
        {
            byte[] send = PacketHelper.Serialize(servicePort,connectionID, 2, null);
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
