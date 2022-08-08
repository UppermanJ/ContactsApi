using Models;
using System.Collections.Generic;
using Models.ServiceLayerResponseWrapper;

namespace Services.Interfaces
{
    public interface IContactService
    {
        ServiceLayerResponseWrapper<Contact> Create(Contact contact);
        ServiceLayerResponseWrapper<IEnumerable<Contact>> GetAll();
        ServiceLayerResponseWrapper<Contact> GetOne(int id);
        ServiceLayerResponse Delete(int id);
        ServiceLayerResponseWrapper<Contact> Update(Contact contact);
        ServiceLayerResponseWrapper<IEnumerable<CallRecord>> GetCallList();
    }
}
