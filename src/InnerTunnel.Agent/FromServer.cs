using System;
using System.Collections.Generic;
using System.Text;
using waxbill;
using waxbill.Protocols;

namespace InnerTunnel.Agent
{
    public class FromServer : TCPServer<FromServerSession>
    {
        /// <summary>
        /// 目标服务端口
        /// </summary>
        public Int32 ServicePort { get; set; }

        public FromServer(Int32 servicePort) : base(new RealtimeProtocol())
        {
            this.ServicePort = servicePort;
        }


        #region Public Interface
        /// <summary>
        /// 关闭会话
        /// </summary>
        /// <param name="connectionID">源客户端ID</param>
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
        /// <param name="connectionID">源客户端ID</param>
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
}
