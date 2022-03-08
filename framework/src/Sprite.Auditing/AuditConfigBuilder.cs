using System.Collections.Generic;

namespace Sprite.Auditing
{
    public class AuditConfigBuilder : IAuditConfigBuilder
    {
        private readonly List<IAuditLogEntryEnricher> _enrichers = new List<IAuditLogEntryEnricher>();

        public IAuditConfigBuilder WithEnricher(IAuditLogEntryEnricher enricher)
        {
            Check.NotNull(enricher, nameof(enricher));
            _enrichers.Add(enricher);
            return this;
        }

        public AuditConfigOptions Build()
        {
            return new AuditConfigOptions()
            {
                Enrichers = _enrichers
            };
        }
    }
}