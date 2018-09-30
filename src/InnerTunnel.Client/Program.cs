using InnerTunnel.Client.Models;
using System;
using ZTImage.Configuration;

namespace InnerTunnel.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ZTImage.Log.Trace.EnableListener(ZTImage.Log.NLog.Instance);
            var config = ConfigHelper.GetInstance<ClientConfigInfo>();
            AgentClient.Instance.Connection("192.168.3.19", config.AgentPort);
            ZTImage.Log.Trace.Info("client is starting...");
            (new System.Threading.ManualResetEvent(false)).WaitOne();
        }
    }
}
