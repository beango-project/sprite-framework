using System;
using System.Collections.Generic;

namespace Sprite.Auditing
{
    public record EntityChangeRecord
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; }

        public DataOperationType OperationType { get; set; }

        public Dictionary<string, object?>? OriginalValues { get; set; }

        public Dictionary<string, object?>? NewValues { get; set; }


        public string? ChangeBy { get; }

        /// <summary>
        /// 更变时间
        /// </summary>
        public DateTime ChangeTime { get; }
    }
}