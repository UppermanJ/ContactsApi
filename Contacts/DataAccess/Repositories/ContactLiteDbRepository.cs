using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.LiteDbInfrastructure;
using DataAccess.LiteDbInfrastructure.Interfaces;
using DataAccess.Repositories.Interfaces;
using LiteDB;
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

        public Contact Update(Contact contact)
        {
            _collection.Update(contact);
            return _collection.FindById(contact.Id);
        }

        public IEnumerable<CallRecord> GetCallList()
        {
            var result =
                _db.Execute($"SELECT $.Name, $.Phone[@.Type = \"Home\"].Number AS Phone FROM {CollectionName} WHERE COUNT($.Phone[@.Type = \"Home\"]) > 0");
            var CallList = new BsonArray(result.ToArray()).ToList().Select(ConvertBsonToCallRecord).ToList();
            return CallList.OrderBy(l => l.Name.Last).ThenBy(f => f.Name.First);
        }

        private static CallRecord ConvertBsonToCallRecord(BsonValue reader)
        {
            return new CallRecord()
            {
                Name = new Name()
                {
                    First = reader["Name"]["First"].ToString().Replace("\"", ""),
                    Middle = reader["Name"]["Middle"].ToString().Replace("\"", ""),
                    Last = reader["Name"]["Last"].ToString().Replace("\"", "")
                },
                Phone = reader["Phone"][0].ToString().Replace("\"", "")
            };
        }
    }
}
