using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.Auditing
{
    public class AuditingManager : IAuditingManager
    {
        [Autowired]
        private Lazy<IServiceProvider> _lazyServiceProvider;

        protected virtual IServiceProvider ServiceProvider => _lazyServiceProvider.Value;

        public AuditConfigOptions Options { get; }


        public AuditingManager(IOptions<AuditConfigOptions> options)
        {
            Options = options.Value;
        }

        public IAuditScope? Current => AmbientAuditScope.Current.AuditScope;

        public IAuditScope Begin(AuditConfigOptions options = null)
        {
            var opts = options ?? Options;
            return InternalBegin(opts);
        }

        public IAuditScope Begin(Action<AuditConfigOptions> options)
        {
            options?.Invoke(Options);
            return InternalBegin(Options);
        }


        /// <summary>
        /// 实际执行开启AuditScope的方法。
        /// 开启并设置环境审计范围的当前范围为新开启的AuditScope。
        /// </summary>
        /// <param name="options">审计配置选项</param>
        /// <returns>审计范围</returns>
        private IAuditScope InternalBegin(AuditConfigOptions options)
        {
            var auditLogEntry = CreateAuditScope(options, new AuditLogEntry());

            var auditScope = AmbientAuditScope.Current.SetAuditScope(auditLogEntry);

            return auditScope;
        }

        private async Task<IAuditScope> InternalBeginAsync(AuditConfigOptions options)
        {
            var auditLogEntry = await CreateAuditScopeAsync(options, new AuditLogEntry());

            var auditScope = AmbientAuditScope.Current.SetAuditScope(auditLogEntry);

            return auditScope;
        }

        /// <summary>
        /// 创建新的AuditScope。同时，为这个范围创建一个上下文和审计日志条目，并调用审计配置选项内的<see cref="IAuditingStartHandler"/>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        protected virtual IAuditScope CreateAuditScope(AuditConfigOptions options, AuditLogEntry log)
        {
            var auditScope = new AuditScope(options, log);

            var context = new AuditScopeContext(null, auditScope);

            foreach (var handler in options.Handlers.OrderBy(handler => handler.Order))
            {
                if (handler is IAuditingStartHandler startHandler)
                {
                    startHandler.Invoke(context);
                }
                else if (handler is IAuditingHandlerAsync startHandlerAsync)
                {
                    startHandlerAsync.InvokeAsync(context).ConfigureAwait(false);
                }
            }

            return auditScope;
        }

        protected virtual async Task<IAuditScope> CreateAuditScopeAsync(AuditConfigOptions options, AuditLogEntry log)
        {
            var auditScope = new AuditScope(options, log);

            var context = new AuditScopeContext(null, auditScope);

            foreach (var handler in options.Handlers.OrderBy(handler => handler.Order))
            {
                if (handler is IAuditingStartHandler startHandler)
                {
                    startHandler.Invoke(context);
                }
                else if (handler is IAuditingHandlerAsync startHandlerAsync)
                {
                    await startHandlerAsync.InvokeAsync(context);
                }
            }

            return auditScope;
        }

        public async Task<IAuditScope> StartAsync()
        {
            return await InternalBeginAsync(Options);
        }

        public void End()
        {
            if (Current == null)
            {
                return;
            }

            var context = new AuditScopeContext(null, Current);

            foreach (var handler in Current.Options.Handlers.OrderBy(handler => handler.Order))
            {
                if (handler is IAuditingEndHandler startHandler)
                {
                    startHandler.Invoke(context);
                }
                else if (handler is IAuditingHandlerAsync startHandlerAsync)
                {
                    startHandlerAsync.InvokeAsync(context).ConfigureAwait(false);
                }
            }
        }

        public Task EndAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}