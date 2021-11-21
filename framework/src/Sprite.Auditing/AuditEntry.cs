using System;
using System.Collections.Generic;

namespace Sprite.Auditing
{
    public class AuditEntry
    {
        /// <summary>
        /// 更变时间
        /// </summary>
        public DateTime ChangeTime { get; }
        
        /// <summary>
        /// 表
        /// </summary>
        public string TableName { get; }
        

        public string EntityId { get; }
        
        public Dictionary<string, object?>? OriginalValues { get; set; }

        public Dictionary<string, object?>? NewValues { get; set; }

        
        public string? UpdateBy { get; }
    }
}