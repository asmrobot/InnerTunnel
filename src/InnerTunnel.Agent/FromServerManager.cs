using InnerTunnel.Agent.Models;
using System;
using System.Collections.Generic;
using System.Text;
using waxbill;
using waxbill.Packets;
using waxbill.Pools;
using waxbill.Protocols;
using waxbill.Sessions;
using ZTImage.Configuration;
using static InnerTunnel.Agent.FromServerManager;

namespace InnerTunnel.Agent
{
    public class FromServerManager
    {

        private Dictionary<Int32, FromServer> ServerItems = new Dictionary<int, FromServer>();

        public void Start()
        {
            var config = ConfigHelper.GetInstance<AgentConfigInfo>();
            if (config.AgentPortMap == null || config.AgentPortMap.Length <= 0)
            {
                throw new Exception("config AgentPortMap error");
            }

            PortMap pm;
            FromServer fsi;
            for (int i = 0; i<config.AgentPortMap.Length; i++)
            {
                pm = config.AgentPortMap[i];
                fsi = new FromServer(pm.ServicePort);
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
            FromServer fsi = this.ServerItems[servicePort];
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
            FromServer fsi = this.ServerItems[servicePort];
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
            FromServer fsi = this.ServerItems[servicePort];
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
        private static FromServerManager instance;
        public static object lockHelper = new object();
        public static FromServerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockHelper)
                    {
                        if (instance == null)
                        {
                            instance = new FromServerManager();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion
    }
}
