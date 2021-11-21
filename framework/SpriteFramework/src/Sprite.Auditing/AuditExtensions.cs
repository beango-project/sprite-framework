using System;

namespace Sprite.Auditing
{
    public static class AuditExtensions
    {
        public static bool WithProperty(this AuditLogEntry auditLogEntry, string propertyName,
            object propertyValue, bool overwrite = false)
        {
            Check.NotNull(auditLogEntry, nameof(auditLogEntry));

            if (auditLogEntry.ExtraProperties.ContainsKey(propertyName) && overwrite == false)
            {
                return false;
            }

            auditLogEntry.ExtraProperties[propertyName] = propertyName;
            return true;
        }

        public static bool WithProperty(this AuditEntry auditEntry, string propertyName,
            object propertyValue, bool overwrite = false)
        {
            // if (null == auditEntry)
            // {
            //     throw new ArgumentNullException(nameof(auditEntry));
            // }
            //
            // if (auditEntry..ContainsKey(propertyName) && overwrite == false)
            // {
            //     return false;
            // }
            //
            // auditEntry.Properties[propertyName] = propertyValue;
            return true;
        }


        public static AuditingBuilder EnrichWithProperty(this AuditingBuilder configBuilder, string propertyName, object value, bool overwrite = false)
        {
            configBuilder.WithEnricher(new AuditLogEntryEnricher(propertyName, value, overwrite));
            return configBuilder;
        }

        public static AuditingBuilder EnrichWithProperty(this AuditingBuilder configBuilder, string propertyName, Func<AuditLogEntry> valueFactory, bool overwrite = false)
        {
            configBuilder.WithEnricher(new AuditLogEntryEnricher(propertyName, valueFactory, overwrite));
            return configBuilder;
        }

        public static AuditingBuilder EnrichWithProperty(this AuditingBuilder configBuilder, string propertyName, object value, Func<AuditLogEntry, bool> predict,
            bool overwrite = false)
        {
            configBuilder.WithEnricher(new AuditLogEntryEnricher(propertyName, _ => value, predict, overwrite));
            return configBuilder;
        }

        public static AuditingBuilder EnrichWithProperty(this AuditingBuilder configBuilder, string propertyName, Func<AuditLogEntry, object> valueFactory,
            Func<AuditLogEntry, bool> predict, bool overwrite = false)
        {
            configBuilder.WithEnricher(new AuditLogEntryEnricher(propertyName, valueFactory, predict, overwrite));
            return configBuilder;
        }
    }
}