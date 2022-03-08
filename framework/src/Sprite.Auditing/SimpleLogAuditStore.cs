using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.Auditing
{
    [Component(ServiceLifetime.Singleton, TryRegister = true)]
    public class SimpleLogAuditStore : IAuditStore
    {
        [Autowired]
        public ILogger<SimpleLogAuditStore> Logger { get; set; }

        public SimpleLogAuditStore()
        {
            Logger = NullLogger<SimpleLogAuditStore>.Instance;
        }

        public void Save(AuditLogEntry logEntry)
        {
            Logger.LogInformation(logEntry.ToString());
        }

        public Task SaveAsync(AuditLogEntry logEntry)
        {
            Logger.LogInformation(logEntry.ToString());
            return Task.CompletedTask;
        }
    }
}