using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace Sprite.Auditing
{
    /// <summary>
    /// 审计配置选项.
    /// </summary>
    public class AuditConfigOptions
    {
        private IReadOnlyCollection<IAuditLogEntryEnricher> _enrichers;

        /// <summary>
        /// Default: true.
        /// </summary>
        public bool IsEnabled { get; set; }

        public IReadOnlyCollection<IAuditLogEntryEnricher> Enrichers
        {
            get => _enrichers.ToImmutableList();
            set => _enrichers = Check.NotNull(value, nameof(value));
        }

        public List<Type> IgnoredTypes { get; private set; }

        public List<KeyValuePair<string, Predicate<Type>>> EntityHistoryPredicates { get; private set;}

        public List<IAuditingHandlerMetadata> Handlers { get; private set; }

        public AuditConfigOptions()
        {
            IsEnabled = true;
            IgnoredTypes = new List<Type>()
            {
                typeof(Stream),
                typeof(Exception)
            };
            EntityHistoryPredicates = new List<KeyValuePair<string, Predicate<Type>>>();
            Handlers = new List<IAuditingHandlerMetadata>()
            {
                new AuditingEnrichWiredHandler()
            };
        }


        public void Clone(AuditConfigOptions options)
        {
            IsEnabled = options.IsEnabled;
            Enrichers = options.Enrichers;
            IgnoredTypes = options.IgnoredTypes;
            Handlers = options.Handlers;
            EntityHistoryPredicates = options.EntityHistoryPredicates;
        }
    }
}