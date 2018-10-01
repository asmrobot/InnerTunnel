using System;
using waxbill.Packets;
using waxbill.Protocols;
using waxbill.Utils;

namespace InnerTunnel.Common
{
    //数据包说明：0x0d,0x0a,4字节网络序长度(网络序,不包含头部的19个字节即：总长-19),1字节操作符，8字节客户端标识，之后为数据
    public class InnerTunnelProtocol: ProtocolBase
    {
        public static InnerTunnelProtocol Define = new InnerTunnelProtocol();
        public InnerTunnelProtocol() : base(19)
        { }

        /// <summary>
        /// 通信长度，不包含协议头的长度
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public virtual int GetSize(byte[] datas)
        {
            return NetworkBitConverter.ToInt32(datas, 2);
        }


        /// <summary>
        /// 解析头部
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="datas"></param>
        /// <param name="count"></param>
        /// <param name="giveupCount"></param>
        /// <returns></returns>
        protected unsafe override bool ParseHeader(Packet _packet, IntPtr datas)
        {
            InnerTunnelPacket packet = _packet as InnerTunnelPacket;
            if (packet == null)
            {
                return false;
            }

            byte* memory = (byte*)datas;
            if (*memory != 0x0d || *(memory + 1) != 0x0a)
            {
                return false;
            }

            packet.ContentLength = NetworkBitConverter.ToInt32(*(memory + 2), *(memory + 3), *(memory + 4), *(memory + 5));
            packet.Action = *(memory + 6);
            packet.ClientIdentity = NetworkBitConverter.ToInt64(*(memory + 7), *(memory + 8), *(memory + 9), *(memory + 10), *(memory + 11), *(memory + 12), *(memory + 13), *(memory + 14));
            packet.ServicePort = NetworkBitConverter.ToInt32(*(memory + 15), *(memory + 16), *(memory + 17), *(memory + 18));
            return true;
        }


        protected unsafe override bool ParseBody(Packet _packet, IntPtr datas, int count, out Int32 giveupCount)
        {
            InnerTunnelPacket packet = _packet as InnerTunnelPacket;
            if (packet == null)
            {
                giveupCount = count;
                return false;
            }
            giveupCount = 0;
            bool result = false;
            if ((count + packet.Count) >= packet.ContentLength)
            {
                giveupCount = packet.ContentLength - (Int32)packet.Count;
                result = true;
            }
            else
            {
                giveupCount = count;
                result = false;
            }
            //todo:保存数据 
            packet.Write(datas, giveupCount);
            return result;
        }

        public override Packet CreatePacket(BufferManager buffer)
        {
            return new InnerTunnelPacket(buffer);
        }
    }
}
