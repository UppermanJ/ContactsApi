using Models;
using System;
using System.Collections.Generic;
using System.Text;
using Models.ServiceLayerResponseWrapper;

namespace Services.Interfaces
{
    public interface IContactService
    {
        ServiceLayerResponseWrapper<Contact> Create(Contact contact);
        ServiceLayerResponseWrapper<IEnumerable<Contact>> GetAll();
        ServiceLayerResponseWrapper<Contact> GetOne(int id);
        ServiceLayerResponse Delete(int id);
    }
}
