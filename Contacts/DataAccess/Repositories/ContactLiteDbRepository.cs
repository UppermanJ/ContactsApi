using System;
using DataAccess.LiteDbInfrastructure;
using DataAccess.LiteDbInfrastructure.Interfaces;
using DataAccess.Repositories.Interfaces;
using Models;

namespace DataAccess.Repositories
{
    public class ContactLiteDbRepository : LiteDbCollection<Contact>, IContactRepository
    {
        public ContactLiteDbRepository(ILiteDbConnection connection): base(connection) {}
        public Contact Create(Contact contact)
        {
            var id = _collection.Insert(contact);
            return _collection.FindById(id);

        }

        public Contact GetSingle(int id)
        {
            throw new NotImplementedException();
        }
    }
}
