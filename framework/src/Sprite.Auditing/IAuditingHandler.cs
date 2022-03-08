using System.Threading.Tasks;

namespace Sprite.Auditing
{
    public interface IAuditingHandler : IAuditingHandlerMetadata
    {
        void Invoke(IAuditScopeContext context);
    }

    public interface IAuditingHandlerAsync : IAuditingHandlerMetadata
    {
        Task InvokeAsync(IAuditScopeContext context);
    }
}