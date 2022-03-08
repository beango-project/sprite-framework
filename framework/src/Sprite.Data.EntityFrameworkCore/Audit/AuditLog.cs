using System;
using System.Collections.Generic;
using Sprite.Common;
using Sprite.Data.Entities;

namespace Sprite.Data.EntityFrameworkCore.Audit
{
    public class AuditLog : IEntityPropertiesEnricher
    {
        public Guid Id { get; internal set; }

        /// <summary>
        /// 执行处理时间
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public int ExecutionDuration { get; set; }

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string ClientIpAddress { get; set; }

        /// <summary>
        /// 客户端名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// 异常
        /// </summary>
        public List<Exception> Exceptions { get; set; }

        public ExtraProperties ExtraProperties { get; set; }

        public virtual ICollection<EntityChangeHistory> EntityChangeHistories { get; set; }

        public AuditLog()
        {
            ExtraProperties = new ExtraProperties();
        }
    }
}