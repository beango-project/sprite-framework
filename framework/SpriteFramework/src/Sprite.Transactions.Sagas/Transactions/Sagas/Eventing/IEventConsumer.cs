using System.Threading.Tasks;

namespace Sprite.Transactions.Sagas.Eventing
{
    /// <summary>
    /// 事件消费者
    /// </summary>
    public interface IEventConsumer<T>
    {
        Task HandleAsync(T t, IProcessContext context);
    }
}