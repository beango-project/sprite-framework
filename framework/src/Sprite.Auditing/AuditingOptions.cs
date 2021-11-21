using System;
using System.Collections.Generic;

namespace Sprite.Auditing
{
    public class AuditingOptions
    {
        public List<IAuditLogEntryEnricher> Enrichers { get; }
        
        public List<Type> IgnoredTypes { get; }
    }
}