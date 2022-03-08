using System.Threading.Tasks;

namespace Sprite.Auditing
{
    /// <summary>
    /// 存储审计数据
    /// </summary>
    public interface IAuditStore
    {
        /// <summary>
        /// 保存审计日志条目
        /// </summary>
        /// <param name="logEntry">审计日志条目</param>
        void Save(AuditLogEntry logEntry);

        /// <summary>
        /// 异步保存审计日志条目
        /// </summary>
        /// <param name="logEntry">审计日志条目</param>
        /// <returns><seealso cref="Task"/></returns>
        Task SaveAsync(AuditLogEntry logEntry);
    }
}