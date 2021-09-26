using System;
using DataAccess.LiteDbInfrastructure.Interfaces;
using LiteDB;

namespace DataAccess.LiteDbInfrastructure
{
    public abstract class LiteDbCollection<T>
    {
        protected ILiteCollection<T> _collection;
        protected LiteDatabase _db;
        public string CollectionName { get; }
        internal LiteDbCollection(ILiteDbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            CollectionName = typeof(T).Name.ToLower();
            _db = connection._db ?? throw new ArgumentNullException(nameof(connection));
            _collection = _db.GetCollection<T>(CollectionName);
        }

    }
}
