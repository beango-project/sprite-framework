using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sprite.Data.Ado;
using Sprite.Tests;
using Xunit;

namespace Sprite.Data.Tests.Persistence
{
    public class Vendor_Tests : SpriteIntegratedTest<DataTestModule>
    {
        private AdoSqlite _ado;

        public Vendor_Tests()
        {
            _ado = ServiceProvider.GetRequiredService<AdoSqlite>();
        }

        [Fact]
        public void Should_Connect()
        {
            _ado.ShouldNotBeNull();
            _ado.DbConnection.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Execute_Command_Create_Table()
        {
            var dbCommand = _ado.CreateCommand();
            dbCommand.CommandText = "CREATE TABLE PERSON(ID INIT PRIMARY KEY NOT NULL,NAME TEXT NOT NULL)";
            var query = dbCommand.ExecuteNonQuery();
            query.ShouldBe(0);
        }

        [Fact]
        public void TX_Tests()
        {
            _ado.CurrentTransaction.ShouldBeNull();

            var transaction1 = _ado.BeginTransaction();

            _ado.CurrentTransaction.ShouldBe(transaction1);
        }

        [Fact]
        public void TX_Nested()
        {
            SqliteConnection conn = new SqliteConnection("data source=:memory:");
            conn.Open();

            var command = conn.CreateCommand();
            command.CommandText = "create table a (b integer primary key autoincrement, c text)";
            command.ExecuteNonQuery();

            var tran1 = conn.BeginTransaction();
            var tran2 = conn.BeginTransaction();

            var command1 = conn.CreateCommand();
            var command2 = conn.CreateCommand();

            command1.Transaction = tran1;
            command2.Transaction = tran2;

            command1.CommandText = "insert into a VALUES (NULL, 'bla1')";
            command2.CommandText = "insert into a VALUES (NULL, 'bla2')";

            command1.ExecuteNonQuery();
            command2.ExecuteNonQuery();

            tran1.Commit();
            tran2.Commit();

            command.CommandText = "select count(*) from a";
            command.ExecuteScalar();
        }
    }
}