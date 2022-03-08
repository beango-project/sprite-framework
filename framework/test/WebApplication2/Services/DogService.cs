using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Sprite.Data.EntityFrameworkCore;
using Sprite.Data.EntityFrameworkCore.Repositories;
using Sprite.Data.Repositories;
using Sprite.Data.Transaction;
using Sprite.Data.Uow;
using Sprite.DependencyInjection;
using Sprite.DependencyInjection.Attributes;
using Z.EntityFramework.Plus;

namespace WebApplication2.Services
{
    public class DogService : ITransientInjection
    {
        private readonly IRepository<Dog, long> _repository;

        // [Autowired]
        // private IRepository<Dog, long> repository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        [Autowired]
        private IDbContextProvider<WebAppDbContext> _dbContextProvider;

        public DogService(IRepository<Dog, long> repository, IUnitOfWorkManager unitOfWorkManager)
        {
            _repository = repository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [Transactional(Propagation.Required)]
        public async Task AddDogs(Expression<Func<Dog, bool>> expression = null)
        {
            var unitOfWork = _unitOfWorkManager.CurrentUow;
            // Console.WriteLine($"Calling{ nameof(AddDogs)}");
            var dog = new Dog() { Name = nameof(AddDogs) };
            await _repository.AddAsync(dog);
            // await unitOfWork.SaveChangesAsync();


            // await AddByTransaction();
            Console.WriteLine($"执行{nameof(GetDogs)},当前仓储DbContext为:{(_dbContextProvider.GetDbContext().ContextId)}");

            await AddByTransaction();
        }

        [Transactional(Propagation.Required)]
        public async Task AddDog_Required()
        {
            var dog = new Dog() { Name = nameof(AddDog_Required) };
            await _repository.AddAsync(dog);
            Console.WriteLine($"执行{nameof(AddDog_Required)},当前仓储DbContext为:{(_dbContextProvider.GetDbContext().ContextId)}");
        }


        [Transactional]
        public async Task<IQueryable<Dog>> GetDogsByRequired()
        {
            return await _repository.GetAllAsync();
        }

        [Transactional(Propagation.RequiresNew)]
        public async Task GetDogsByRequiredNew()
        {
            await _repository.GetAllAsync();
        }

        [Transactional(Propagation.NotSupported)]
        public async Task GetDogsByNotSupported()
        {
            await _repository.GetAllAsync();
        }

        [Transactional(Propagation = Propagation.NotSupported)]
        public async Task AddDog_NotSupported()
        {
            var dog = new Dog() { Name = nameof(AddDog_NotSupported) };
            await _repository.AddAsync(dog);
            Console.WriteLine($"执行{nameof(AddDog_NotSupported)},当前仓储DbContext为:{(_dbContextProvider.GetDbContext().ContextId)}");
        }

        [Transactional(Propagation.Supports)]
        private async Task AddByTransaction()
        {
            // Console.WriteLine($"Calling{ nameof(AddByTransaction)}");
            var dog = new Dog() { Name = nameof(AddByTransaction) };
            await _repository.AddAsync(dog);
            Console.WriteLine($"执行{nameof(AddByTransaction)},当前仓储DbContext为:{(_dbContextProvider.GetDbContext().ContextId)}");
            // throw new Exception("手动错误");
        }

        [Transactional]
        private async Task AddNotTransaction()
        {
            // Console.WriteLine($"执行{nameof(GetAll)},当前仓储DbContext为:{(_dbContextProvider.GetDbContext().ContextId)}");

            Console.WriteLine($"Calling{nameof(AddNotTransaction)}");

            await _repository.AddAsync(null);
        }

        [Transactional(Propagation.Supports)]
        public async Task<IQueryable<Dog>> GetDogs()
        {
            return (await _repository.GetAllAsync()).OrderBy(d=>d.CreateTime);
        }

        [Transactional]
        public async Task DeleteById(long id)
        {
            var res = await _repository.DeleteAsync(x => x.Id == id);
            if (res != 1)
            {
                throw new Exception("删除失败");
            }
        }
        
        public async Task UpdateById(long id)
        {
            var dog = await _repository.FindAsync(id);
            await _repository.UpdateAsync(dog);
            // await _repository.Where(x => x.Id == id).UpdateAsync(x => new
            // {
            //     Name=x.Name+1
            // });
        }
    }
}