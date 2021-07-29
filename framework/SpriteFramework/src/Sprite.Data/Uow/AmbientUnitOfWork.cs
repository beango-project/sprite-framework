using System.Threading;
using Sprite.DependencyInjection;
using Sprite.DependencyInjection.Attributes;

namespace Sprite.Data.Uow
{
    [Export(typeof(IAmbientUnitOfWork), typeof(IUnitOfWorkAccessor))]
    public class AmbientUnitOfWork : IAmbientUnitOfWork, ISingletonInjection
    {
        private readonly AsyncLocal<IUnitOfWork> _currentUow;

        public AmbientUnitOfWork()
        {
            _currentUow = new AsyncLocal<IUnitOfWork>();
        }

        public IUnitOfWork UnitOfWork => _currentUow.Value;

        public void SetUnitOfWork(IUnitOfWork unitOfWork)
        {
            _currentUow.Value = unitOfWork;
        }
    }
}