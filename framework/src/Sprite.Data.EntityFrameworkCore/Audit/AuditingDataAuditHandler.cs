using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Auditing;
using Sprite.Data.Transaction;
using Sprite.Data.Uow;

namespace Sprite.Data.EntityFrameworkCore.Audit
{
    public class AuditingDataAuditHandler : IAuditingEndHandler
    {
        private DbContext _dbContext;
        public int Order => 999;

        public void Invoke(IAuditScopeContext context)
        {
            // using (var serviceScope = context.ServiceProvider.CreateScope())
            // {
            //     var unitOfWorkManager = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
            //     var auditingManager = serviceScope.ServiceProvider.GetRequiredService<IAuditingManager>();
            //     var currentAuditScope = auditingManager.Current;
            //     if (currentAuditScope != null && currentAuditScope.IsDisposed == false)
            //     {
            //         using (var unitOfWork = unitOfWorkManager.Begin(new TransactionOptions()
            //                {
            //                    Propagation = Propagation.RequiresNew
            //                }))
            //         {
            //             var auditLogEntry = currentAuditScope.Log;
            //             var auditLog = new AuditLog()
            //             {
            //                 ExecutionTime = auditLogEntry.ExecutionTime,
            //                 ClientIpAddress = auditLogEntry.ClientIpAddress,
            //                 ClientName = auditLogEntry.ClientName,
            //                 BrowserInfo = auditLogEntry.BrowserInfo,
            //                 HttpMethod = auditLogEntry.HttpMethod,
            //                 Exceptions = auditLogEntry.Exceptions,
            //                 ExtraProperties = auditLogEntry.ExtraProperties,
            //             };
            //             _dbContext.Set<AuditLog>().Add(auditLog);
            //             unitOfWork.SaveChanges();
            //             unitOfWork.OnDisposed += (_, _) => { serviceScope.Dispose(); };
            //         }
            //     }
            // }
        }
    }
}