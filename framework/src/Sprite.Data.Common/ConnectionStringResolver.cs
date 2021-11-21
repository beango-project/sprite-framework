using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Sprite.Data
{
    public class ConnectionStringResolver : IConnectionStringResolver
    {
        public ConnectionStringResolver(IOptionsMonitor<DbConnectionOptions> options)
        {
            Options = options.CurrentValue;
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