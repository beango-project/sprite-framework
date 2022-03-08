using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Auditing;
using Sprite.Data.EntityFrameworkCore.Audit;

namespace Sprite.Data.EntityFrameworkCore.Extensions
{
    public class DbContextDataAuditExtension : IDbContextOptionsExtension
    {
        public void ApplyServices(IServiceCollection services)
        {
            // services.PostConfigure<AuditConfigOptions>(options =>
            // {
            //     if (!options.Handlers.Any(handler => handler is AuditingDataAuditHandler))
            //     {
            //         options.Handlers.Add(new AuditingDataAuditHandler());
            //     }
            // });
        }

        public void Validate(IDbContextOptions options)
        {
            // throw new System.NotImplementedException();
        }

        public DbContextOptionsExtensionInfo Info { get; }
    }
}