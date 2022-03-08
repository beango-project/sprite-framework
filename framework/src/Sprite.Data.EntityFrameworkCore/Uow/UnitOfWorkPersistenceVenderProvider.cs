using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sprite.Data.EntityFrameworkCore.Persistence;
using Sprite.Data.Persistence;
using Sprite.Data.Transaction;
using Sprite.Data.Uow;
using Sprite.DependencyInjection.Attributes;
using TransactionOptions = Sprite.Data.Transaction.TransactionOptions;

namespace Sprite.Data.EntityFrameworkCore.Uow
{
    public class UnitOfWorkPersistenceVenderProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContextBase<TDbContext>
    {
        private readonly IConnectionStringResolver _connectionStringResolver;

        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<TDbContext> _logger;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UnitOfWorkPersistenceVenderProvider(IServiceProvider serviceProvider, IUnitOfWorkManager unitOfWorkManager, IConnectionStringResolver connectionStringResolver,
            ILogger<TDbContext> logger)
        {
            _serviceProvider = serviceProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _connectionStringResolver = connectionStringResolver;
            _logger = logger;
        }

        public TDbContext GetDbContext()
        {
            var dbContextMetadata = CreateOrGetDbContext();
            HandleTransaction(dbContextMetadata.unitOfWork, dbContextMetadata.vendor, dbContextMetadata.dbContext, dbContextMetadata.connectionString);
            return dbContextMetadata.dbContext;
        }


        public async Task<TDbContext> GetDbContextAsync()
        {
            var dbContextMetadata = await ValueTask.FromResult(CreateOrGetDbContext());
            await HandleTransactionAsync(dbContextMetadata.unitOfWork, dbContextMetadata.vendor, dbContextMetadata.dbContext, dbContextMetadata.connectionString);
            return dbContextMetadata.dbContext;
        }

        private string ResolveConnectionStringName()
        {
            var connectionStringName = ConnectionStringAttribute.GetConnectionStringName<TDbContext>();

            return _connectionStringResolver.GetConnectionString(connectionStringName);
        }

        private (IUnitOfWork unitOfWork, TDbContext dbContext, IVendor vendor, string connectionString) CreateOrGetDbContext()
        {
            var connectionString = ResolveConnectionStringName();

            var unitOfWorkManager = _serviceProvider.GetRequiredService<IUnitOfWorkManager>();
            var unitOfWork = unitOfWorkManager.CurrentUow;
            if (unitOfWork == null)
            {
                throw new Exception("DbContext need unit of work");
            }

            string vendorKey = $"EfCore_{typeof(TDbContext).FullName}_{connectionString}";
            var vendor = unitOfWork.FindVendor(vendorKey);
            TDbContext dbContext;
            if (vendor == null) //if vendor is null ,we will create new DbContext and set 
            {
                var serviceScope = _serviceProvider.CreateScope();
                dbContext = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();
                dbContext.ServiceProvider = serviceScope.ServiceProvider;
                _logger.LogDebug("未找到DbContext缓存，开启新的{DbContext}", dbContext);
                vendor = new EfCorePersistence<TDbContext>(dbContext);

                if (unitOfWork is UnitOfWork)
                {
                    unitOfWork.OnDisposed += (_, _) =>
                    {
                        _logger.LogDebug("DbContext is Disposed {DbContext}", dbContext);
                        vendor.Dispose();
                        serviceScope.Dispose();
                    };
                }
                else if (unitOfWork is VirtualUnitOfWork virtualUnitOfWork)
                {
                    virtualUnitOfWork.BaseUow.OnDisposed += (_, _) =>
                    {
                        _logger.LogDebug("DbContext is Disposed {DbContext}", dbContext);
                        vendor.Dispose();
                        serviceScope.Dispose();
                    };
                }

                //TODO:添加持久化提供商，会激活工作单元并覆盖之前的配置是否正确？
                unitOfWork.GetOrAddVendor(vendorKey, vendor);
            }
            else
            {
                dbContext = ((IEfCorePersistence<TDbContext>)vendor).DbContext;
                _logger.LogDebug("找到DbContext缓存，进行重用{DbContext}", dbContext);
            }

            return (unitOfWork, dbContext, vendor, connectionString);
        }


        private void HandleTransaction(IUnitOfWork unitOfWork, IVendor vendor, TDbContext dbContext, string connectionString)
        {
            string txKey = $"{vendor.DbConnection}_{connectionString}";
            DbTransaction dbTransaction;
            switch (unitOfWork.Options.Propagation)
            {
                case Propagation.Auto:
                    break;

                case Propagation.Required:
                    // if (!unitOfWork.IsSupportTransaction)
                    // {
                    //     throw new Exception($"{nameof(TransactionPropagation.Required)} request transaction,but current unit of work non support transaction");
                    // }

                    dbTransaction = unitOfWork.FindDbTransaction(txKey);
                    if (dbTransaction == null && dbContext.Database.CurrentTransaction == null) //找不到事务并且当前DbContext的事务为空时，才开启并添加新的事务
                    {
                        unitOfWork.AddDbTransaction(txKey,
                            unitOfWork.Options.IsolationLevel.HasValue
                                ? dbContext.Database.BeginTransaction(unitOfWork.Options.IsolationLevel.Value).GetDbTransaction()
                                : dbContext.Database.BeginTransaction().GetDbTransaction());
                    }
                    else
                    {
                        if (dbContext.Database.CurrentTransaction == null) //如果在工作单元内找事务，且当前事务为空时 
                        {
                            // is relational db
                            if (dbContext.Database.GetService<IDbContextTransactionManager>() is IRelationalTransactionManager) //是关系型数据库
                            {
                                if (dbContext.Database.GetDbConnection() == dbTransaction.Connection) //如果连接串相同则使用工作单元内的事务（事务共享）
                                {
                                    dbContext.Database.UseTransaction(dbTransaction);
                                }
                                else //不同则开启新的事务
                                {
                                    if (unitOfWork.Options.IsolationLevel.HasValue)
                                    {
                                        dbContext.Database.BeginTransaction(unitOfWork.Options.IsolationLevel.Value);
                                    }
                                    else
                                    {
                                        dbContext.Database.BeginTransaction();
                                    }
                                }
                            }
                            else
                            {
                                //非关系型数据库则单独开启各自的事务
                                dbContext.Database.BeginTransaction();
                            }
                        }
                    }

                    break;

                case Propagation.RequiresNew:

                    if (unitOfWork.IsSupportTransaction)
                    {
                        dbTransaction = unitOfWork.FindDbTransaction(txKey);
                        if (dbTransaction == null && dbContext.Database.CurrentTransaction == null)
                        {
                            unitOfWork.AddDbTransaction(txKey,
                                unitOfWork.Options.IsolationLevel.HasValue
                                    ? dbContext.Database.BeginTransaction(unitOfWork.Options.IsolationLevel.Value).GetDbTransaction()
                                    : dbContext.Database.BeginTransaction().GetDbTransaction());
                        }
                        //TODO : 如果当前事务不等于工作单元内的事务？
                        // else
                        // {
                        //     if (dbTransaction == dbContext.Database.CurrentTransaction.GetDbTransaction())
                        //     {
                        //     }
                        // }
                    }
                    else
                    {
                        throw new Exception("当前事务传播类型需要事务，但工作单元不支持事务");
                    }

                    break;

                case Propagation.Supports:

                    //如果工作单元是事务的，就代表当前或许可以设置事务
                    if (unitOfWork.IsSupportTransaction)
                    {
                        dbTransaction = unitOfWork.FindDbTransaction(txKey);
                        if (dbTransaction != null) //如果有事务就使用当前事务
                        {
                            // is relational db
                            if (dbContext.Database.GetService<IDbContextTransactionManager>() is IRelationalTransactionManager) //是关系型数据库
                            {
                                if (dbContext.Database.GetDbConnection() == dbTransaction.Connection) //如果连接串相同则使用工作单元内的事务（事务共享）
                                {
                                    dbContext.Database.UseTransaction(dbTransaction);
                                }
                            }
                            // 不是关系型数据库则不共享也不设置事务
                        }
                    }

                    break;

                case Propagation.Mandatory:

                    dbTransaction = unitOfWork.FindDbTransaction(txKey);
                    if (dbTransaction == null)
                    {
                        if (dbContext.Database.CurrentTransaction == null)
                        {
                            throw new Exception($"{nameof(Propagation.Mandatory)} request transaction,but there is currently no transaction");
                        }

                        unitOfWork.AddDbTransaction(txKey, dbContext.Database.CurrentTransaction.GetDbTransaction());
                    }
                    else
                    {
                        // is relational db
                        if (dbContext.Database.GetService<IDbContextTransactionManager>() is IRelationalTransactionManager) //是关系型数据库
                        {
                            if (dbContext.Database.GetDbConnection() == dbTransaction.Connection) //如果连接串相同则使用工作单元内的事务（事务共享）
                            {
                                dbContext.Database.UseTransaction(dbTransaction);
                            }
                            else //不同则代表没有事务
                            {
                                throw new Exception($"{nameof(Propagation.Mandatory)} request transaction,but there is currently no transaction");
                            }
                        }
                    }

                    break;

                case Propagation.NotSupported:

                    //如果当前工作单元是支持事务的，则开启一个新的非事务的工作单元和DbContext，并挂起之前的
                    if (unitOfWork.IsSupportTransaction || unitOfWork.HasTransaction || dbContext.Database.CurrentTransaction != null)
                    {
                        var newUnitOfWork = _unitOfWorkManager.Begin(new TransactionOptions()
                        {
                            Propagation = Propagation.NotSupported
                        });

                        var serviceScope = _serviceProvider.CreateScope();
                        dbContext = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();
                        dbContext.ServiceProvider = serviceScope.ServiceProvider;
                        dbContext.Database.AutoTransactionsEnabled = false; //close transaction
                        vendor = new EfCorePersistence<TDbContext>(dbContext);

                        newUnitOfWork.OnDisposed += (_, _) =>
                        {
                            vendor.Dispose();
                            serviceScope.Dispose();
                        };
                        newUnitOfWork.GetOrAddVendor($"EfCore_{typeof(TDbContext).FullName}_{connectionString}", vendor);
                    }

                    break;

                case Propagation.Never:

                    if (unitOfWork.HasTransaction || dbContext.Database.CurrentTransaction != null)
                    {
                        throw new Exception("需要非事务地执行，而当前事务不为空");
                    }

                    dbContext.Database.AutoTransactionsEnabled = false;
                    break;
            }
        }

        private async Task HandleTransactionAsync(IUnitOfWork unitOfWork, IVendor vendor, TDbContext dbContext, string connectionString)
        {
            string txKey = $"{vendor.DbConnection}_{connectionString}";
            DbTransaction dbTransaction;
            switch (unitOfWork.Options.Propagation)
            {
                case Propagation.Auto:
                    break;

                case Propagation.Required:
                    // if (!unitOfWork.IsSupportTransaction)
                    // {
                    //     throw new Exception($"{nameof(TransactionPropagation.Required)} request transaction,but current unit of work non support transaction");
                    // }

                    dbTransaction = unitOfWork.FindDbTransaction(txKey);
                    if (dbTransaction == null && dbContext.Database.CurrentTransaction == null) //找不到事务并且当前DbContext的事务为空时，才开启并添加新的事务
                    {
                        unitOfWork.AddDbTransaction(txKey,
                            unitOfWork.Options.IsolationLevel.HasValue
                                ? (await dbContext.Database.BeginTransactionAsync(unitOfWork.Options.IsolationLevel.Value)).GetDbTransaction()
                                : (await dbContext.Database.BeginTransactionAsync()).GetDbTransaction());
                    }
                    else
                    {
                        if (dbContext.Database.CurrentTransaction == null) //如果在工作单元内找事务，且当前事务为空时 
                        {
                            // is relational db
                            if (dbContext.Database.GetService<IDbContextTransactionManager>() is IRelationalTransactionManager) //是关系型数据库
                            {
                                if (dbContext.Database.GetDbConnection() == dbTransaction.Connection) //如果连接串相同则使用工作单元内的事务（事务共享）
                                {
                                    await dbContext.Database.UseTransactionAsync(dbTransaction);
                                }
                                else //不同则开启新的事务
                                {
                                    if (unitOfWork.Options.IsolationLevel.HasValue)
                                    {
                                        await dbContext.Database.BeginTransactionAsync(unitOfWork.Options.IsolationLevel.Value);
                                    }
                                    else
                                    {
                                        await dbContext.Database.BeginTransactionAsync();
                                    }
                                }
                            }
                            else
                            {
                                //非关系型数据库则单独开启各自的事务
                                await dbContext.Database.BeginTransactionAsync();
                            }
                        }
                    }

                    break;

                case Propagation.RequiresNew:

                    if (unitOfWork.IsSupportTransaction)
                    {
                        dbTransaction = unitOfWork.FindDbTransaction(txKey);
                        if (dbTransaction == null && dbContext.Database.CurrentTransaction == null)
                        {
                            unitOfWork.AddDbTransaction(txKey,
                                unitOfWork.Options.IsolationLevel.HasValue
                                    ? (await dbContext.Database.BeginTransactionAsync(unitOfWork.Options.IsolationLevel.Value)).GetDbTransaction()
                                    : (await dbContext.Database.BeginTransactionAsync()).GetDbTransaction());
                        }
                        //TODO : 如果当前事务不等于工作单元内的事务？
                        // else
                        // {
                        //     if (dbTransaction == dbContext.Database.CurrentTransaction.GetDbTransaction())
                        //     {
                        //     }
                        // }
                    }
                    else
                    {
                        throw new Exception("当前事务传播类型需要事务，但工作单元不支持事务");
                    }

                    break;

                case Propagation.Supports:

                    //如果工作单元是事务的，就代表当前或许可以设置事务
                    if (unitOfWork.IsSupportTransaction)
                    {
                        dbTransaction = unitOfWork.FindDbTransaction(txKey);
                        if (dbTransaction != null) //如果有事务就使用当前事务
                        {
                            // is relational db
                            if (dbContext.Database.GetService<IDbContextTransactionManager>() is IRelationalTransactionManager) //是关系型数据库
                            {
                                if (dbContext.Database.GetDbConnection() == dbTransaction.Connection) //如果连接串相同则使用工作单元内的事务（事务共享）
                                {
                                    await dbContext.Database.UseTransactionAsync(dbTransaction);
                                }
                            }
                            // 不是关系型数据库则不共享也不设置事务
                        }
                    }

                    break;

                case Propagation.Mandatory:

                    dbTransaction = unitOfWork.FindDbTransaction(txKey);
                    if (dbTransaction == null)
                    {
                        if (dbContext.Database.CurrentTransaction == null)
                        {
                            throw new Exception($"{nameof(Propagation.Mandatory)} request transaction,but there is currently no transaction");
                        }

                        unitOfWork.AddDbTransaction(txKey, dbContext.Database.CurrentTransaction.GetDbTransaction());
                    }
                    else
                    {
                        // is relational db
                        if (dbContext.Database.GetService<IDbContextTransactionManager>() is IRelationalTransactionManager) //是关系型数据库
                        {
                            if (dbContext.Database.GetDbConnection() == dbTransaction.Connection) //如果连接串相同则使用工作单元内的事务（事务共享）
                            {
                                await dbContext.Database.UseTransactionAsync(dbTransaction);
                            }
                            else //不同则代表没有事务
                            {
                                throw new Exception($"{nameof(Propagation.Mandatory)} request transaction,but there is currently no transaction");
                            }
                        }
                    }

                    break;

                case Propagation.NotSupported:

                    //如果当前工作单元是支持事务的，则开启一个新的非事务的工作单元和DbContext，并挂起之前的
                    if (unitOfWork.IsSupportTransaction || unitOfWork.HasTransaction || dbContext.Database.CurrentTransaction != null)
                    {
                        var newUnitOfWork = _unitOfWorkManager.Begin(new TransactionOptions()
                        {
                            Propagation = Propagation.NotSupported
                        });
                        var serviceScope = _serviceProvider.CreateScope();
                        dbContext = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();
                        dbContext.ServiceProvider = serviceScope.ServiceProvider;
                        dbContext.Database.AutoTransactionsEnabled = false; //close transaction
                        vendor = new EfCorePersistence<TDbContext>(dbContext);
                        var newVendor = vendor;
                        newUnitOfWork.OnDisposed += (_, _) =>
                        {
                            newVendor.Dispose();
                            serviceScope.Dispose();
                        };
                        newUnitOfWork.GetOrAddVendor($"EfCore_{typeof(TDbContext).FullName}_{connectionString}", vendor);
                    }

                    break;

                case Propagation.Never:

                    if (unitOfWork.HasTransaction || dbContext.Database.CurrentTransaction != null)
                    {
                        throw new Exception("需要非事务地执行，而当前事务不为空");
                    }

                    dbContext.Database.AutoTransactionsEnabled = false;
                    break;
            }
        }
    }
}