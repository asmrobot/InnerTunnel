using System;

namespace InnerTunnel.Client
{
    class Program
    {
        static void Main(string[] args)
        {

            ZTImage.Log.Trace.EnableListener(ZTImage.Log.NLog.Instance);
            AgentClient.Instance.Connection("127.0.0.1", 23889);
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
