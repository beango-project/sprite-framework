using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Sprite.Auditing
{
    /// <summary>
    /// 用于对系统和业务进行审计，可以设置和管理审计的范围和审计的内容，并将范围内涉及到的审计数据持久化。
    /// </summary>
    public interface IAuditingManager
    {
        /// <summary>
        /// 获取当前的AuditScope，如果不存在，此项可能为空。
        /// </summary>
        [CanBeNull]
        IAuditScope? Current { get; }

        /// <summary>
        /// 开启AuditScope。
        /// </summary>
        /// <param name="options">审计配置选项<see cref="AuditConfigOptions"/></param>
        /// <returns>AuditScope</returns>
        IAuditScope Begin(AuditConfigOptions options = null);


        /// <summary>
        /// 开启AuditScope。
        /// </summary>
        /// <param name="options">审计配置选项<see cref="AuditConfigOptions"/></param>
        /// <returns>AuditScope</returns>
        IAuditScope Begin(Action<AuditConfigOptions> options);

        /// <summary>
        /// 异步开启AuditScope。
        /// </summary>
        /// <param name="options">审计配置选项<see cref="AuditConfigOptions"/></param>
        /// <returns>AuditScope</returns>
        Task<IAuditScope> StartAsync();

        /// <summary>
        /// 结束审计
        /// </summary>
        void End();

        /// <summary>
        /// 异步结束审计
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task EndAsync();
    }
}