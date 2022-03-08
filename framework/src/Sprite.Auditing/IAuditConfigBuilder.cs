namespace Sprite.Auditing
{
    public interface IAuditConfigBuilder
    {
        IAuditConfigBuilder WithEnricher(IAuditLogEntryEnricher enricher);

        AuditConfigOptions Build();
    }
}