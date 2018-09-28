using InnerTunnel.Agent.Sessions;
using System;
using waxbill;
using waxbill.Protocols;

namespace InnerTunnel.Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            TCPServer<FromSession> fromServer = new TCPServer<FromSession>(new RealtimeProtocol());
            TCPServer<ToSession> toServer = new TCPServer<ToSession>(new ZTProtocol());
            fromServer.Start("0.0.0.0", 13889);
            toServer.Start("0.0.0.0", 23889);
            Console.WriteLine("server is started");
            Console.ReadKey();
        }
    }
}
