using JetBrains.Annotations;

namespace Sprite.Data.Uow
{
    public interface IUnitOfWorkAccessor
    {
        [CanBeNull]
        IUnitOfWork UnitOfWork { get; }

        void SetUnitOfWork([CanBeNull] IUnitOfWork unitOfWork);
    }
}