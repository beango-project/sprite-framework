using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sprite.Auditing;
using Sprite.Common;
using Sprite.Data.Entities;

namespace Sprite.Data.EntityFrameworkCore.Audit
{
    public class EntityChangeHistory : IEntityPropertiesEnricher
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public virtual string TableName { get; internal set; }

        public virtual string EntityId { get; }

        public virtual DataOperationType OperationType { get; set; }

        public Dictionary<string, object?> KeyValues { get; }
        
        /// <summary>
        /// 原始值
        /// </summary>
        public virtual Dictionary<string, object?>? OriginalValues { get; set; }

        /// <summary>
        /// 新值
        /// </summary>
        public virtual Dictionary<string, object?>? NewValues { get; set; }

        /// <summary>
        /// 更变者
        /// </summary>

        public virtual string? ChangeBy { get; }

        /// <summary>
        /// 更变时间
        /// </summary>
        public virtual DateTime ChangeTime { get; }

        public ExtraProperties ExtraProperties { get; }

        public EntityChangeHistory()
        {
            KeyValues = new Dictionary<string, object?>();
            ExtraProperties = new ExtraProperties();
        }
    }

    internal sealed class InternalEntityChangeHistory : EntityChangeHistory
    {
        public List<PropertyEntry>? TemporaryProperties { get; set; }

        public InternalEntityChangeHistory(EntityEntry entityEntry)
        {
            TableName = entityEntry.Metadata.GetTableName();

            if (entityEntry.Properties.Any(x=>x.IsTemporary))
            {
                TemporaryProperties = new List<PropertyEntry>();
            }

            switch (entityEntry.State)
            {
                case EntityState.Added:
                    OperationType = DataOperationType.Created;
                    OriginalValues = new Dictionary<string, object?>();
                    break;
                case EntityState.Modified:
                    OperationType = DataOperationType.Updated;
                    OriginalValues = new Dictionary<string, object?>();
                    NewValues = new Dictionary<string, object?>();
                    break;
                case EntityState.Deleted:
                    OperationType = DataOperationType.Updated;
                    OriginalValues = new Dictionary<string, object?>();
                    break;
            }
            
            foreach (var propertyEntry in entityEntry.Properties)
            {
                // if (AuditConfig.AuditConfigOptions.PropertyFilters.Any(f => f.Invoke(entityEntry, propertyEntry) == false))
                // {
                //     continue;
                // }

                if (propertyEntry.IsTemporary)
                {
                    TemporaryProperties!.Add(propertyEntry);
                    continue;
                }

                var columnName = propertyEntry.Metadata.GetColumnBaseName();
                if (propertyEntry.Metadata.IsPrimaryKey())
                {
                    KeyValues[columnName] = propertyEntry.CurrentValue;
                }
                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        NewValues![columnName] = propertyEntry.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        OriginalValues![columnName] = propertyEntry.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (propertyEntry.IsModified)
                        {
                            OriginalValues![columnName] = propertyEntry.OriginalValue;
                            NewValues![columnName] = propertyEntry.CurrentValue;
                        }
                        break;
                }
            }
        }
    }
}