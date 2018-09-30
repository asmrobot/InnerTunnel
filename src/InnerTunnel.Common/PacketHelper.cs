using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace InnerTunnel.Common
{
    /// <summary>
    /// 包帮助类
    /// </summary>
    public class PacketHelper
    {
        /// <summary>
        /// 序列化成数据包
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="action"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static byte[] Serialize(Int64 connectionID,byte action,byte[] datas)
        {
            Int32 len = 0;
            if (datas != null && datas.Length > 0)
            {
                len = datas.Length;
            }
            len += 15;
            byte[] sendDatas = new byte[len];
            sendDatas[0] = 0x0d;
            sendDatas[1] = 0x0a;

            Int32 protocolLen=IPAddress.HostToNetworkOrder(len - 15);
            byte[] lenBytes = BitConverter.GetBytes(protocolLen);
            sendDatas[2] = lenBytes[0];
            sendDatas[3] = lenBytes[1];
            sendDatas[4] = lenBytes[2];
            sendDatas[5] = lenBytes[3];
            sendDatas[6] = action;

            Int64 conLen = IPAddress.HostToNetworkOrder(connectionID);
            byte[] conBytes = BitConverter.GetBytes(conLen);
            
            sendDatas[7] = conBytes[0];
            sendDatas[8] = conBytes[1];
            sendDatas[9] = conBytes[2];
            sendDatas[10] = conBytes[3];
            sendDatas[11] = conBytes[4];
            sendDatas[12] = conBytes[5];
            sendDatas[13] = conBytes[6];
            sendDatas[14] = conBytes[7];

            if (datas != null && datas.Length > 0)
            {
                Buffer.BlockCopy(datas, 0, sendDatas, 15, datas.Length);
            }
            
            return sendDatas;
        }

    }
}
