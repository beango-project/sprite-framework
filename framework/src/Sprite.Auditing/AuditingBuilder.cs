using System.Collections.Generic;

namespace Sprite.Auditing
{
    public class AuditingBuilder
    {
        public IList<IAuditLogEntryEnricher> Enrichers { get; }

        public AuditingBuilder WithEnricher(IAuditLogEntryEnricher enricher)
        {
            Check.NotNull(enricher, nameof(enricher));

            Enrichers.Add(enricher);
            return this;
        }
    }
}