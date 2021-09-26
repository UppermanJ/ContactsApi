using System;
using DataAccess.LiteDbInfrastructure;
using DataAccess.LiteDbInfrastructure.Interfaces;
using NUnit.Framework;

namespace DataAccess.Tests.Integration.Infrastructure
{
    public abstract class LiteDbTestBase
    {
        protected ILiteDbConnection _connection;

        [OneTimeSetUp]
        public void Connect()
        {
            _connection = new LiteDbConnection(Environment.GetEnvironmentVariable("Coding_Challenge_Upperman"));
        }

        [SetUp]
        public void BeginTransaction()
        {
            _connection._db.BeginTrans();
        }

        [TearDown]
        public void RollbackTransaction()
        {
            _connection._db.Rollback();
        }
        
    }
}
