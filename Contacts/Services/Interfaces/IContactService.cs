using Models;
using System;
using System.Collections.Generic;
using System.Text;
using Models.ServiceLayerResponseWrapper;

namespace Services.Interfaces
{
    public interface IContactService
    {
        ServiceLayerResponse<Contact> Create(Contact contact);
    }
}
