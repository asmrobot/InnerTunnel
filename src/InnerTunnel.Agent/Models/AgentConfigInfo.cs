using System;
using System.Collections.Generic;
using System.Text;
using ZTImage.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace InnerTunnel.Agent.Models
{
    [ConfigPath("configs","agent.config")]
    public class AgentConfigInfo
    {
        /// <summary>
        /// 来源端口和服务端口的映射
        /// </summary>
        [XmlArray]
        public PortMap[] AgentPortMap;
        
        
        /// <summary>
        /// 代理端口
        /// </summary>
        public Int32 AgentPort { get; set; }
    }

    [Serializable]
    public class PortMap
    {
        /// <summary>
        /// 来源端口
        /// </summary>
        [XmlAttribute]
        public Int32 FromPort { get; set; }

        /// <summary>
        /// 服务端口
        /// </summary>
        [XmlAttribute]
        public Int32 ServicePort { get; set; }

    }
}
