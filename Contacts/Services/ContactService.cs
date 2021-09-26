using System;
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

        public ServiceLayerResponse<Contact> Create(Contact contact)
        {
            var response = new ServiceLayerResponse<Contact>();
            response.AddValidationErrors(_contactValidator.Validate(contact));
            return response.HasErrors ? response : response.SetData(_contactRepository.Create(contact));
        }
    }
}
