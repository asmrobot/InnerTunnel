using InnerTunnel.Agent.Models;
using System;
using waxbill;
using waxbill.Packets;
using waxbill.Protocols;
using waxbill.Sessions;
using ZTImage.Configuration;

namespace InnerTunnel.Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "agent";
            ZTImage.Log.Trace.EnableListener(ZTImage.Log.NLog.Instance);
            waxbill.Trace.SetMessageWriter(ZTImage.Log.NLog.Instance.Error, ZTImage.Log.NLog.Instance.Info);

            StartServer();
            //StartSSHTest();
            (new System.Threading.ManualResetEvent(false)).WaitOne();
        }
        
        private static void StartServer()
        {
            var config = ConfigHelper.GetInstance<AgentConfigInfo>();
            //FromServer.Instance.Start("0.0.0.0", config.FromPort);
            FromServerManager.Instance.Start();
            AgentServer.Instance.Start("0.0.0.0", config.AgentPort);
            ZTImage.Log.Trace.Info("server is started");
        }

        private static void StartSSHTest()
        {
            TCPClient client = new TCPClient(new RealtimeProtocol());
            client.OnDisconnected += Client_OnDisconnected;
            client.OnReceived += Client_OnReceive;
            client.Connect("192.168.3.222", 22);
            ZTImage.Log.Trace.Info("connect is starting");
        }

        private static void Client_OnReceive(TCPClient client,SessionBase session, Packet packet)
        {
            byte[] datas = packet.Read();
            ZTImage.Log.Trace.Info(System.Text.Encoding.ASCII.GetString(datas));
        }

        private static void Client_OnDisconnected(TCPClient client, waxbill.Sessions.SessionBase session, CloseReason exception)
        {
            ZTImage.Log.Trace.Info("ssh is disconnect");
            
        }
    }
}
