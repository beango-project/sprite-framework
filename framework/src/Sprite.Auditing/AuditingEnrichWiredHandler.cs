using Sprite.DependencyInjection.Attributes;

namespace Sprite.Auditing
{
    public class AuditingEnrichWiredHandler : AuditingStartHandler
    {
        public override int Order => 0;

        public override void Invoke(IAuditScopeContext context)
        {
            foreach (var entryEnricher in context.AuditScope.Options.Enrichers)
            {
                entryEnricher.Enrich(context.AuditScope.Log);
            }
        }
    }
}