using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.Data
{
    [Register(ServiceLifetime.Transient)]
    public class ConnectionStringResolver : IConnectionStringResolver
    {
        public ConnectionStringResolver(IOptionsSnapshot<DbConnectionOptions> options)
        {
            Options = options.Value;
        }

        protected DbConnectionOptions Options { get; }

        public virtual string GetConnectionString(string connectionStringName = null)
        {
            if (!string.IsNullOrEmpty(connectionStringName))
            {
                Options.ConnectionStrings.TryGetValue(connectionStringName, out var connString);
                if (!string.IsNullOrEmpty(connString))
                {
                    return connString;
                }
            }

            return Options.ConnectionStrings.Default;
        }

        public virtual Task<string> GetConnectionStringAsync(string connectionStringName = null)
        {
            return Task.FromResult(GetConnectionString(connectionStringName));
        }
    }
}