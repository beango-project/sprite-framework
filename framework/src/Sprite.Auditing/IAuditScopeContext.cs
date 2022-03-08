using System;

namespace Sprite.Auditing
{
    public interface IAuditScopeContext
    {
        IServiceProvider ServiceProvider { get; }

        IAuditScope AuditScope { get; }

   
    }

    public class AuditScopeContext : IAuditScopeContext
    {
        public AuditScopeContext(IServiceProvider serviceProvider, IAuditScope auditScope)
        {
            ServiceProvider = serviceProvider;
            AuditScope = auditScope;
        }

        public IServiceProvider ServiceProvider { get; }
        public IAuditScope AuditScope { get; }
    }
}