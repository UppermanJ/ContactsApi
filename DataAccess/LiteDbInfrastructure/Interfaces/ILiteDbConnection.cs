using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace DataAccess.LiteDbInfrastructure.Interfaces
{
    public interface ILiteDbConnection
    {
        public LiteDatabase _db { get; }
    }
}
