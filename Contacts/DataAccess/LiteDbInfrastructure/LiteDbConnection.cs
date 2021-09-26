using System;
using DataAccess.LiteDbInfrastructure.Interfaces;
using LiteDB;

namespace DataAccess.LiteDbInfrastructure
{
    public class LiteDbConnection: ILiteDbConnection
    {
        public LiteDatabase _db { get; }

        public LiteDbConnection(string desiredDirectory)
        {
            if (String.IsNullOrWhiteSpace(desiredDirectory))
            {
                throw new ArgumentNullException(nameof(desiredDirectory));
            }
            _db = new LiteDatabase($"{desiredDirectory}\\LiteDb.db");
        }
    }
}
