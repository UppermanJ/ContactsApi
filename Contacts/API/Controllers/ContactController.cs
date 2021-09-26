using System;
using API.DTOs;
using API.Helpers;
using API.Helpers.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders.Testing;
using Models;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactController> _logger;
        private readonly IRequestAccessor _contextInteractor;
        public ContactController(ILogger<ContactController> logger, IContactService contactService, IMapper mapper, IRequestAccessor contextInteractor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _contextInteractor = contextInteractor ?? throw new ArgumentNullException(nameof(contextInteractor));
        }
        
        [HttpPost]
        public IActionResult Create([FromBody] ContactDTO contact)
        {
            try
            {
                var mappedContact = _mapper.Map<Contact>(contact);
                var serviceResponse = _contactService.Create(mappedContact);
                var mappedResponseData = _mapper.Map<ExistingContactDTO>(serviceResponse.Data);
                return JsonResponseBuilder<ExistingContactDTO>.ForCreate
                    .SetData(mappedResponseData)
                    .SetInformation(serviceResponse.Messages)
                    .Build();
            }
            catch(Exception e)
            {
                _logger.LogCritical(e, e.Message);
                return JsonResponseBuilder<ExistingContactDTO>.ReturnFailure(_contextInteractor.GetTraceId(Request));
            }
        }
    }
}
