using System;
using waxbill.Packets;
using waxbill.Pools;
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
        protected unsafe override bool ParseHeader(Packet _packet, ArraySegment<byte> datas)
        {
            InnerTunnelPacket packet = _packet as InnerTunnelPacket;
            if (packet == null)
            {
                return false;
            }

            
            if (datas.Array[datas.Offset] != 0x0d || datas.Array[datas.Offset+1] != 0x0a)
            {
                return false;
            }

            packet.ContentLength = NetworkBitConverter.ToInt32(
                datas.Array[datas.Offset + 2], 
                datas.Array[datas.Offset + 3], 
                datas.Array[datas.Offset + 4], 
                datas.Array[datas.Offset + 5]);

            packet.Action = datas.Array[datas.Offset+6];
            packet.ClientIdentity = NetworkBitConverter.ToInt64(
                datas.Array[datas.Offset + 7],
                datas.Array[datas.Offset + 8],
                datas.Array[datas.Offset + 9],
                datas.Array[datas.Offset + 10],
                datas.Array[datas.Offset + 11],
                datas.Array[datas.Offset + 12],
                datas.Array[datas.Offset + 13],
                datas.Array[datas.Offset + 14]);

            packet.ServicePort = NetworkBitConverter.ToInt32(
                datas.Array[datas.Offset + 15],
                datas.Array[datas.Offset + 16],
                datas.Array[datas.Offset + 17],
                datas.Array[datas.Offset + 18]);
            return true;
        }


        protected unsafe override bool ParseBody(Packet _packet, ArraySegment<byte> datas,  out Int32 giveupCount)
        {
            InnerTunnelPacket packet = _packet as InnerTunnelPacket;
            if (packet == null)
            {
                giveupCount = datas.Count;
                return false;
            }
            giveupCount = 0;
            bool result = false;
            if ((datas.Count + packet.Count) >= packet.ContentLength)
            {
                giveupCount = packet.ContentLength - (Int32)packet.Count;
                result = true;
            }
            else
            {
                giveupCount = datas.Count;
                result = false;
            }
            //todo:保存数据 
            packet.Write(new ArraySegment<byte>(datas.Array,datas.Offset, giveupCount));
            return result;
        }

        public override Packet CreatePacket(PacketBufferPool buffer)
        {
            return new InnerTunnelPacket(buffer);
        }
    }
}
