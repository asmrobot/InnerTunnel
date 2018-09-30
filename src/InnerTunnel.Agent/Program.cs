using System;
using waxbill;
using waxbill.Protocols;

namespace InnerTunnel.Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            ZTImage.Log.Trace.EnableListener(ZTImage.Log.NLog.Instance);

            FromServer.Instance.Start("0.0.0.0", 13889);
            ToServer.Instance.Start("0.0.0.0", 23889);

            Console.WriteLine("server is started");
            Console.ReadKey();
        }
    }
}
