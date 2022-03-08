using System.Collections.Generic;

namespace Sprite.Auditing
{
    public static class AuditLogEntryExtensions
    {
        // public static AuditLogEntry EnabledComments(this AuditLogEntry auditLogEntry)
        // {
        //     auditLogEntry.ExtraProperties.TryAdd("Comments", new List<string>());
        //     return auditLogEntry;
        // }
        //
        //
        public static List<string>? GetComments(this AuditLogEntry auditLogEntry)
        {
            return (List<string>?)auditLogEntry.ExtraProperties.GetValueOrDefault("Comments");
        }
    }
}