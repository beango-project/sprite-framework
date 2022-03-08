using System;
using System.Collections.Generic;

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

            auditLogEntry.ExtraProperties[propertyName] = propertyValue;
            return true;
        }

        public static bool WithProperty(this AuditLogEntry auditLogEntry, string propertyName,
            Func<AuditLogEntry, object> propertyValueFactory, bool overwrite = false)
        {
            if (null == auditLogEntry)
            {
                throw new ArgumentNullException(nameof(auditLogEntry));
            }

            if (auditLogEntry.ExtraProperties.ContainsKey(propertyName) && overwrite == false)
            {
                return false;
            }

            lock (auditLogEntry)
            {
                auditLogEntry.ExtraProperties[propertyName] = propertyValueFactory?.Invoke(auditLogEntry);
            }

            return true;
        }

        public static IAuditConfigBuilder AddComments(this IAuditConfigBuilder configBuilder)
        {
            return EnrichWithProperty(configBuilder, "Comments", new List<string>());
        }


        public static IAuditConfigBuilder EnrichWithProperty(this IAuditConfigBuilder configBuilder, string propertyName, object value, bool overwrite = false)
        {
            configBuilder.WithEnricher(new AuditLogEntryEnricher(propertyName, value, overwrite));
            return configBuilder;
        }

        public static IAuditConfigBuilder EnrichWithProperty(this IAuditConfigBuilder configBuilder, string propertyName, Func<AuditLogEntry> valueFactory, bool overwrite = false)
        {
            configBuilder.WithEnricher(new AuditLogEntryEnricher(propertyName, valueFactory, overwrite));
            return configBuilder;
        }

        public static IAuditConfigBuilder EnrichWithProperty(this IAuditConfigBuilder configBuilder, string propertyName, object value, Func<AuditLogEntry, bool> predict,
            bool overwrite = false)
        {
            configBuilder.WithEnricher(new AuditLogEntryEnricher(propertyName, _ => value, predict, overwrite));
            return configBuilder;
        }

        public static IAuditConfigBuilder EnrichWithProperty(this IAuditConfigBuilder configBuilder, string propertyName, Func<AuditLogEntry, object> valueFactory,
            Func<AuditLogEntry, bool> predict, bool overwrite = false)
        {
            configBuilder.WithEnricher(new AuditLogEntryEnricher(propertyName, valueFactory, predict, overwrite));
            return configBuilder;
        }
    }
}