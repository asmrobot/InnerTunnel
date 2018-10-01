using InnerTunnel.Agent.Models;
using System;
using System.Collections.Generic;
using System.Text;
using waxbill;
using waxbill.Packets;
using waxbill.Protocols;
using waxbill.Sessions;
using ZTImage.Configuration;
using static InnerTunnel.Agent.FromServer;

namespace InnerTunnel.Agent
{
    public class FromServer
    {

        private Dictionary<Int32, FromServerItem> ServerItems = new Dictionary<int, FromServerItem>();

        public void Start()
        {
            var config = ConfigHelper.GetInstance<AgentConfigInfo>();
            if (config.AgentPortMap == null || config.AgentPortMap.Length <= 0)
            {
                throw new Exception("config AgentPortMap error");
            }

            PortMap pm;
            FromServerItem fsi;
            for (int i = 0; i<config.AgentPortMap.Length; i++)
            {
                pm = config.AgentPortMap[i];
                fsi = new FromServerItem(pm.ServicePort);
                fsi.Start("0.0.0.0", pm.FromPort);
                ServerItems.Add(pm.ServicePort, fsi);
            }
        }
        
        /// <summary>
        /// 关闭会话
        /// </summary>
        /// <param name="connectionID"></param>
        public void CloseSession(Int32 servicePort,Int64 connectionID)
        {
            if (!this.ServerItems.ContainsKey(servicePort))
            {
                return;
            }
            FromServerItem fsi = this.ServerItems[servicePort];
            fsi.CloseSession(connectionID);
        }

        /// <summary>
        /// 向指定会话发送消息
        /// </summary>
        /// <param name="connectionID"></param>
        public void SendData(Int32 servicePort,Int64 connectionID,byte[] datas)
        {
            if (!this.ServerItems.ContainsKey(servicePort))
            {
                return;
            }
            FromServerItem fsi = this.ServerItems[servicePort];
            fsi.SendData(connectionID,datas);
        }

        /// <summary>
        /// 通道关闭
        /// </summary>
        public void TunnelClose(Int32 servicePort)
        {
            if (!this.ServerItems.ContainsKey(servicePort))
            {
                return;
            }
            FromServerItem fsi = this.ServerItems[servicePort];
            fsi.TunnelClose();
        }

        /// <summary>
        /// 通道关闭
        /// </summary>
        public void TunnelCloseAll()
        {
            foreach (var item in ServerItems)
            {
                item.Value.TunnelClose();
            }
        }

        #region singleton
        private static FromServer instance;
        public static object lockHelper = new object();
        public static FromServer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockHelper)
                    {
                        if (instance == null)
                        {
                            instance = new FromServer();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion


        public class FromServerItem : TCPServer<FromItemSession>
        {
            public Int32 ServicePort { get; set; }

            public FromServerItem(Int32 servicePort) : base(new RealtimeProtocol())
            {
                this.ServicePort = servicePort;
            }


            #region Public Interface
            /// <summary>
            /// 关闭会话
            /// </summary>
            /// <param name="connectionID"></param>
            public void CloseSession(Int64 connectionID)
            {
                var session = this.GetSession(connectionID);
                if (session == null)
                {
                    return;
                }

                if (session.IsClosed)
                {
                    return;
                }
                session.Close(CloseReason.RemoteClose);
            }

            /// <summary>
            /// 向指定会话发送消息
            /// </summary>
            /// <param name="connectionID"></param>
            public void SendData(Int64 connectionID, byte[] datas)
            {
                var session = this.GetSession(connectionID);
                if (session == null)
                {
                    return;
                }

                if (session.IsClosed)
                {
                    return;
                }
                session.Send(datas);
            }

            /// <summary>
            /// 通道关闭
            /// </summary>
            public void TunnelClose()
            {
                var sessions = this.GetSessions();
                foreach (var session in sessions)
                {
                    session.Close(CloseReason.RemoteClose);
                }
            }
            #endregion
        }
        public class FromItemSession : ServerSession
        {
            protected override void OnConnected()
            {
                FromServerItem fsi = this.Monitor as FromServerItem;
                if (fsi == null)
                {
                    return;
                }
                ZTImage.Log.Trace.Info("connect:" + this.RemoteEndPoint.ToString());
                AgentServer.Instance.SendConnect(fsi.ServicePort,this.ConnectionID);
            }

            protected override void OnDisconnected(CloseReason reason)
            {
                FromServerItem fsi = this.Monitor as FromServerItem;
                if (fsi == null)
                {
                    return;
                }
                ZTImage.Log.Trace.Info("disconnect:" + this.RemoteEndPoint.ToString());
                AgentServer.Instance.SendDisconnect(fsi.ServicePort,this.ConnectionID);
            }

            protected override void OnReceived(Packet packet)
            {
                FromServerItem fsi = this.Monitor as FromServerItem;
                if (fsi == null)
                {
                    return;
                }
                byte[] datas = packet.Read();
                AgentServer.Instance.SendDatas(fsi.ServicePort,this.ConnectionID,datas);
            }

            protected override void OnSended(PlatformBuf packet, bool result)
            { }
        }
    }
}
