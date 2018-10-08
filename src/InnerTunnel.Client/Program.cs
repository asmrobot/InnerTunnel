using InnerTunnel.Client.Models;
using System;
using ZTImage.Configuration;

namespace InnerTunnel.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "client";
            ZTImage.Log.Trace.EnableListener(ZTImage.Log.NLog.Instance);
            waxbill.Trace.SetMessageWriter(ZTImage.Log.NLog.Instance.Error, ZTImage.Log.NLog.Instance.Info);

            var config = ConfigHelper.GetInstance<ClientConfigInfo>();
            AgentClient.Instance.Connection(config.AgentIP, config.AgentPort);
            ZTImage.Log.Trace.Info("client is starting...");
            (new System.Threading.ManualResetEvent(false)).WaitOne();
        }
    }
}
