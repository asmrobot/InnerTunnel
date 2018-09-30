using System;
using System.Collections.Generic;
using System.Text;
using waxbill;
using waxbill.Packets;
using waxbill.Protocols;
using waxbill.Sessions;
using static InnerTunnel.Agent.FromServer;

namespace InnerTunnel.Agent
{
    public class FromServer: TCPServer<FromSession>
    {
        private FromServer():base(new RealtimeProtocol())
        {}

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
        public void SendData(Int64 connectionID,byte[] datas)
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
            var sessions=this.GetSessions();
            foreach (var session in sessions)
            {
                session.Close(CloseReason.RemoteClose);
            }
        }
        #endregion

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

        public class FromSession : ServerSession
        {
            protected override void OnConnected()
            {
                ZTImage.Log.Trace.Info("connect:" + this.RemoteEndPoint.ToString());
                ToServer.Instance.SendConnect(this.ConnectionID);
            }

            protected override void OnDisconnected(CloseReason reason)
            {
                ZTImage.Log.Trace.Info("disconnect:" + this.RemoteEndPoint.ToString());
                ToServer.Instance.SendDisconnect(this.ConnectionID);
            }

            protected override void OnReceived(Packet packet)
            {
                byte[] datas = packet.Read();
                ZTImage.Log.Trace.Info("receive:" + System.Text.Encoding.ASCII.GetString(datas));
                ToServer.Instance.SendDatas(this.ConnectionID,datas);
            }

            protected override void OnSended(PlatformBuf packet, bool result)
            { }
        }
    }
}
