using System;
using System.Collections.Generic;
using Sprite.Data.Entities;

namespace Sprite.Auditing
{
    [Serializable]
    public class AuditLogEntry : IEntityPropertiesEnricher
    {
        /// <summary>
        /// 执行处理时间
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        public int ExecutionDuration { get; set; }

        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }

        public List<Exception> Exceptions { get; }

        public ExtraProperties ExtraProperties { get; }
    }
}