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
        /// 代理IP
        /// </summary>
        public String AgentIP { get; set; }

        /// <summary>
        /// 代理端口，即agent服务的ToPort
        /// </summary>
        public Int32 AgentPort{ get; set; }

        /// <summary>
        /// 服务IP
        /// </summary>
        public String ServiceIP { get; set; }
        
    }
}
