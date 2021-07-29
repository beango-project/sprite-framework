using System.Threading.Tasks;

namespace Sprite.Data
{
    public interface IConnectionStringResolver
    {
        string GetConnectionString(string connectionStringName = null);

        Task<string> GetConnectionStringAsync(string connectionStringName = null);
    }
}