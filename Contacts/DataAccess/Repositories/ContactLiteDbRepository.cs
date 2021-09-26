using System;
using System.Collections.Generic;
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

        public Contact GetOne(int id)
        {
            return _collection.FindById(id);
        }

        public IEnumerable<Contact> GetAll()
        {
            return _collection.FindAll();
        }

        public void Delete(int id)
        {
            _collection.Delete(id);
        }
    }
}
