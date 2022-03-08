namespace Sprite.Auditing
{
    public abstract class AuditingStartHandler : IAuditingStartHandler
    {
        public abstract int Order { get; }
        public abstract void Invoke(IAuditScopeContext context);
    }
}