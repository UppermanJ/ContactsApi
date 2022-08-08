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
            _logger.LogTrace($"Entered {nameof(Create)} with arguments: {nameof(contact)}: {contact}");
            var response = new ServiceLayerResponseWrapper<Contact>();
            response.AddValidationErrors(_contactValidator.Validate(contact));
            if (false == response.HasErrors)
            {
                response.SetData(_contactRepository.Create(contact));
            }
            _logger.LogTrace($"Exiting {nameof(Create)} with values: {nameof(response)}: {response}");
            return response;
        }

        public ServiceLayerResponseWrapper<IEnumerable<Contact>> GetAll()
        {
            _logger.LogTrace($"Entered {nameof(GetAll)}");
            var response = new ServiceLayerResponseWrapper<IEnumerable<Contact>>().SetData(_contactRepository.GetAll());
            _logger.LogTrace($"Exiting {nameof(GetAll)} with values: {nameof(response)}: {response}");
            return response;
        }

        public ServiceLayerResponseWrapper<Contact> GetOne(int id)
        {
            _logger.LogTrace($"Entered {nameof(GetOne)} with arguments: {nameof(id)}: {id}");
            var response = new ServiceLayerResponseWrapper<Contact>();
            var contact = _contactRepository.GetOne(id);
            if (contact == null)
            {
                response.AddInformation<NotFound>("No contact found with that Id");
                _logger.LogTrace($"Exiting {nameof(GetOne)} with values: {nameof(response)}: {response}");
                return response;
            }
            response.SetData(contact);
            _logger.LogTrace($"Exiting {nameof(GetOne)} with values: {nameof(response)}: {response}");
            return response;
        }

        public ServiceLayerResponse Delete(int id)
        {
            _logger.LogTrace($"Entered {nameof(Delete)} with arguments: {nameof(id)}: {id}");
            var response = new ServiceLayerResponse();
            _contactRepository.Delete(id);
            _logger.LogTrace($"Exiting {nameof(Delete)} with values: {nameof(response)}: {response}");
            return response;
        }

        public ServiceLayerResponseWrapper<Contact> Update(Contact contact)
        {
            _logger.LogTrace($"Entered {nameof(Update)} with arguments: {nameof(contact)}: {contact}");
            var response = new ServiceLayerResponseWrapper<Contact>();
            var foundContact = _contactRepository.GetOne(contact.Id);
            if (foundContact == null)
            {
                response.AddInformation<NotFound>("No contact found with that Id");
                _logger.LogTrace($"Exiting {nameof(Update)} with values: {nameof(response)}: {response}");
                return response;
            }

            if (foundContact.IsEqual(contact))
            {
                response.SetData(foundContact).AddInformation<NoActionNeeded>("Existing is an exact match");
                _logger.LogTrace($"Exiting {nameof(Update)} with values: {nameof(response)}: {response}");
                return response;
            }

            response.AddValidationErrors(_contactValidator.Validate(contact));
            if (response.HasErrors == false)
            {
                response.SetData(_contactRepository.Update(contact));
            }
            _logger.LogTrace($"Exiting {nameof(Update)} with values: {nameof(response)}: {response}");
            return response;
        }

        public ServiceLayerResponseWrapper<IEnumerable<CallRecord>> GetCallList()
        {
            _logger.LogTrace($"Entered {nameof(GetCallList)}");
            var response = new ServiceLayerResponseWrapper<IEnumerable<CallRecord>>().SetData(_contactRepository.GetCallList());
            _logger.LogTrace($"Exiting {nameof(Update)} with values: {nameof(response)}: {response}");
            return response;
        }
    }
}
