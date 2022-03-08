using System;
using System.Collections.Generic;
using System.Linq;
using Sprite.Common;
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
        public List<Exception> Exceptions { get; }

        public List<string> Comments => _lazyComments.Value;


        public ExtraProperties ExtraProperties { get; }

        private Lazy<List<string>> _lazyComments { get; }

        public AuditLogEntry()
        {
            ExtraProperties = new ExtraProperties();
            _lazyComments = new Lazy<List<string>>();
        }

        public AuditLogEntry(params string[] comments)
        {
            ExtraProperties = new ExtraProperties();
            if (comments != null && comments.Length > 0)
            {
                _lazyComments = new Lazy<List<string>>(comments.ToList());
            }
        }

        public string ToJsonString()
        {
            return new AuditJsonSerializer().Serialize(this);
        }
    }
}