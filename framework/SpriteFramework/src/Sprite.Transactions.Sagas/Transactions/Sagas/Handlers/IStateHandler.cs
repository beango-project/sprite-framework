using System.Threading.Tasks;

namespace Sprite.Transactions.Sagas.Handlers
{
    public interface IStateHandler
    {
        Task HandleAsync(IProcessContext context);
    }
}