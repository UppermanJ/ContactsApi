using System.Collections.Generic;
using Models;

namespace DataAccess.Repositories.Interfaces
{
    public interface IContactRepository
    {
        Contact Create(Contact contact);
        Contact GetOne(int id);
        IEnumerable<Contact> GetAll();
        void Delete(int id);
        Contact Update(Contact contact);
        IEnumerable<CallRecord> GetCallList();
    }
}
