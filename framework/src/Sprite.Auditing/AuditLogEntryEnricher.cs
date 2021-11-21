using System;
using JetBrains.Annotations;
using Sprite.Common;

namespace Sprite.Auditing
{
    public class AuditLogEntryEnricher : Enricher<AuditLogEntry>, IAuditLogEntryEnricher
    {
        public AuditLogEntryEnricher(string propertyName, object propertyValue, bool overwrite = false) : base(propertyName, propertyValue, overwrite)
        {
        }

        public AuditLogEntryEnricher(string propertyName, Func<AuditLogEntry, object> propertyValueFactory, bool overwrite = false) : base(propertyName, propertyValueFactory,
            overwrite)
        {
        }

        public AuditLogEntryEnricher(string propertyName, Func<AuditLogEntry, object> propertyValueFactory, [CanBeNull] Func<AuditLogEntry, bool>? propertyPredict,
            bool overwrite = false) : base(propertyName, propertyValueFactory, propertyPredict, overwrite)
        {
        }

        protected override Action<AuditLogEntry, string, Func<AuditLogEntry, object>, bool>? EnrichAction => (auditLogEntry, propertyName, valueFactory, overwrite) =>
            auditLogEntry.WithProperty(propertyName, valueFactory, overwrite);
    }
}