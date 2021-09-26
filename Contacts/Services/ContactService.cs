using System;
using System.Collections.Generic;
using DataAccess.Repositories.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Models;
using Models.ServiceLayerResponseWrapper;
using Services.Extensions;
using Services.Interfaces;

namespace Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IValidator<Contact> _contactValidator;
        private ILogger<ContactService> _logger;

        public ContactService(ILogger<ContactService> logger, IContactRepository contactRepository,
            IValidator<Contact> validator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            _contactValidator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public ServiceLayerResponseWrapper<Contact> Create(Contact contact)
        {
            var response = new ServiceLayerResponseWrapper<Contact>();
            response.AddValidationErrors(_contactValidator.Validate(contact));
            return response.HasErrors ? response : response.SetData(_contactRepository.Create(contact));
        }

        public ServiceLayerResponseWrapper<IEnumerable<Contact>> GetAll()
        {
            return new ServiceLayerResponseWrapper<IEnumerable<Contact>>().SetData(_contactRepository.GetAll());
        }

        public ServiceLayerResponseWrapper<Contact> GetOne(int id)
        {
            var response = new ServiceLayerResponseWrapper<Contact>();
            var contact = _contactRepository.GetOne(id);
            if (contact == null)
            {
                return response.AddInformation<NotFound>("No contact found with that Id");
            }
            return response.SetData(contact);
        }

        public ServiceLayerResponse Delete(int id)
        {
            var response = new ServiceLayerResponse();
            _contactRepository.Delete(id);
            return response;
        }
    }
}
