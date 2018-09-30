using System;
using System.Collections.Generic;
using System.Text;
using ZTImage.Configuration;

namespace InnerTunnel.Agent.Models
{
    [ConfigPath("configs","agent.config")]
    public class AgentConfigInfo
    {
        /// <summary>
        /// 来源端口
        /// </summary>
        public Int32 FromPort { get; set; }

        /// <summary>
        /// 转发至端口
        /// </summary>
        public Int32 ToPort { get; set; }
    }
}
