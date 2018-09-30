using System;
using System.Collections.Generic;
using System.Text;
using ZTImage.Log;
using ZTImage.Configuration;

namespace InnerTunnel.Client.Models
{

    [ConfigPath("configs","client.config")]
    public class ClientConfigInfo
    {
        /// <summary>
        /// 代理端口，即agent服务的ToPort
        /// </summary>
        public Int32 AgentPort{ get; set; }

        /// <summary>
        /// 提供服务的端口
        /// </summary>
        public Int32 ServicePort { get; set; }
    }
}
