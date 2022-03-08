using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sprite.Data.Persistence;
using Sprite.Data.Tests.Persistence;
using Sprite.Data.Transaction;
using Sprite.Data.Uow;
using Sprite.Tests;
using Xunit;

namespace Sprite.Data.Tests.Uow
{
    public class Uow_Tx_Propagation_Tests : SpriteIntegratedTest<DataTestModule>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public Uow_Tx_Propagation_Tests()
        {
            _unitOfWorkManager = ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
        }

        [Fact]
        public void Should_Create_Nested_Uow()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.CurrentUow.ShouldNotBeNull();
                _unitOfWorkManager.CurrentUow.ShouldBe(uow);

                using (var nestedUow = _unitOfWorkManager.Begin())
                {
                    _unitOfWorkManager.CurrentUow.ShouldNotBeNull();
                    _unitOfWorkManager.CurrentUow.ShouldBe(nestedUow);
                    _unitOfWorkManager.CurrentUow.ShouldBeOfType<VirtualUnitOfWork>();
                    _unitOfWorkManager.CurrentUow.Outer.ShouldBeNull();
                    ((VirtualUnitOfWork)nestedUow).BaseUow.ShouldBe(uow);
                }
            }

            using (var uow = _unitOfWorkManager.Begin())
            {
            }

            _unitOfWorkManager.CurrentUow.ShouldBeNull();
        }

        [Fact]
        public void Should_Create_Virtual_Uow_With_Tx_Propagation_Request_Of_Non_Tx()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.CurrentUow.ShouldNotBeNull();
                _unitOfWorkManager.CurrentUow.ShouldBe(uow);

                using (var nestedUow = _unitOfWorkManager.Begin())
                {
                    _unitOfWorkManager.CurrentUow.ShouldNotBeNull();
                    _unitOfWorkManager.CurrentUow.ShouldBe(nestedUow);
                    _unitOfWorkManager.CurrentUow.ShouldBeOfType<VirtualUnitOfWork>();
                    _unitOfWorkManager.CurrentUow.Outer.ShouldBeNull();
                    ((VirtualUnitOfWork)nestedUow).BaseUow.ShouldBe(uow);
                }
            }

            _unitOfWorkManager.CurrentUow.ShouldBeNull();
        }

        [Fact]
        public void Should_Create_Virtual_Uow_With_Tx_Propagation_RequestNew_Of_Non_Tx()
            // public void Should_Create_New_Uow_With_Tx_Propagation_RequestNew()
        {
            var txOpts = new TransactionOptions()
            {
                Propagation = Propagation.RequiresNew
            };
            using (var uow = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.CurrentUow.ShouldNotBeNull();
                _unitOfWorkManager.CurrentUow.ShouldBe(uow);

                using (var newUow = _unitOfWorkManager.Begin(txOpts))
                {
                    _unitOfWorkManager.CurrentUow.ShouldNotBeNull();
                    _unitOfWorkManager.CurrentUow.ShouldBe(newUow);
                    _unitOfWorkManager.CurrentUow.ShouldBeOfType<UnitOfWork>();
                    _unitOfWorkManager.CurrentUow.Outer.ShouldBe(uow);

                    using (var newUow2 = _unitOfWorkManager.Begin(txOpts))
                    {
                        _unitOfWorkManager.CurrentUow.ShouldNotBeNull();
                        _unitOfWorkManager.CurrentUow.ShouldBe(newUow2);
                        _unitOfWorkManager.CurrentUow.ShouldBeOfType<UnitOfWork>();
                        _unitOfWorkManager.CurrentUow.Outer.ShouldBe(newUow);
                    }

                    _unitOfWorkManager.CurrentUow.ShouldBe(newUow);
                }
            }

            _unitOfWorkManager.CurrentUow.ShouldBeNull();
        }

        [Fact]
        public void Should_Create_New_Uow_With_Tx_Propagation_Supports()
        {
            var txOpts = new TransactionOptions()
            {
                Propagation = Propagation.Supports
            };


            var vendor = ServiceProvider.GetRequiredService<AdoSqlite>();
            // var vendor2 = ServiceProvider.GetRequiredService<AdoSqlite>();
            
            var str = $"{vendor.DbConnection}";
            
            List<SqlConnection> dbConnections = new List<SqlConnection>();
            var sqlConnection = new SqlConnection();
            var sqlConnection2 = new SqlConnection();
            Assert.Equal(sqlConnection,sqlConnection);
            // var tx1 = sqlConnection.BeginTransaction();
            // transactions.Add(tx1);
            // var tx2 = sqlConnection2.BeginTransaction();
      


            using (var uow = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.CurrentUow.ShouldNotBeNull();
                _unitOfWorkManager.CurrentUow.ShouldBe(uow);
                uow.HasTransaction.ShouldBeFalse();
                AddVendorInUow(_unitOfWorkManager.CurrentUow, "Ado_SQLite", vendor);

                vendor.CurrentTransaction.ShouldBeNull();

                var dbTx = vendor.BeginTransaction();
                vendor.CurrentTransaction.ShouldBe(dbTx);
                uow.HasTransaction.ShouldBeTrue();
            }

            _unitOfWorkManager.CurrentUow.ShouldBeNull();
        }

        private void AddVendorInUow(IUnitOfWork unitOfWork, string key, IVendor vendor)
        {
            unitOfWork.GetOrAddVendor(key, vendor);
        }
    }
}