using System;
using System.Collections.Generic;
using System.Text;
using waxbill.Packets;
using waxbill.Pools;
using waxbill.Utils;

namespace InnerTunnel.Common
{
    
    //数据包说明：0x0d,0x0a,4字节网络序长度,1字节操作符，4字节客户端标识，之后为数据
    public class InnerTunnelPacket:Packet
    {
        public InnerTunnelPacket(PacketBufferPool buffer):base(buffer)
        {

        }

        /// <summary>
        /// 数据长度
        /// </summary>
        public Int32 ContentLength { get; set; }

        /// <summary>
        /// 操作符
        /// 0:连接，1发送数据，2：断开连接,3:通道关闭
        /// </summary>
        public byte Action { get; set; }


        /// <summary>
        /// 客户端标识
        /// </summary>
        public Int64 ClientIdentity { get; set; }

        /// <summary>
        /// 服务端口
        /// </summary>
        public Int32 ServicePort { get; set; }

    }
}
