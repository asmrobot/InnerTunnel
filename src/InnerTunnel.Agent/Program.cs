using InnerTunnel.Agent.Models;
using System;
using waxbill;
using waxbill.Protocols;
using ZTImage.Configuration;

namespace InnerTunnel.Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            ZTImage.Log.Trace.EnableListener(ZTImage.Log.NLog.Instance);

            var config = ConfigHelper.GetInstance<AgentConfigInfo>();
            FromServer.Instance.Start("0.0.0.0", config.FromPort);
            ToServer.Instance.Start("0.0.0.0", config.ToPort);
            ZTImage.Log.Trace.Info("server is started");
            (new System.Threading.ManualResetEvent(false)).WaitOne();
        }
    }
}
