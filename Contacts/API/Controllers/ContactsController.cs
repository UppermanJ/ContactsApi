using System;
using System.Collections.Generic;
using API.DTOs;
using API.Helpers;
using API.Helpers.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactsController> _logger;
        private readonly IRequestAccessor _contextInteractor;

        public ContactsController(ILogger<ContactsController> logger, IContactService contactService, IMapper mapper,
            IRequestAccessor contextInteractor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _contextInteractor = contextInteractor ?? throw new ArgumentNullException(nameof(contextInteractor));
        }

        [HttpGet]
        [ProducesDefaultResponseType(typeof(IEnumerable<ExistingContactDTO>))]
        public IActionResult GetAll()
        {
            try
            {
                _logger.LogTrace($"Entered {nameof(GetAll)}");
                var serviceResponse = _contactService.GetAll();
                var mappedContacts = _mapper.Map<IEnumerable<ExistingContactDTO>>(serviceResponse.Data);
                return ApiResponseBuilder.ForRead
                    .SetData(mappedContacts)
                    .SetInformation(serviceResponse.Messages)
                    .Build();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, e.Message);
                return ApiResponseBuilder.ReturnFailure(_contextInteractor.GetTraceId(Request));
            }
        }

        [HttpPost]
        [ProducesDefaultResponseType(typeof(ExistingContactDTO))]
        public IActionResult Create([FromBody] ContactDTO contact)
        {
            try
            {
                _logger.LogTrace($"Entered {nameof(Create)} with arguments: {nameof(contact)}: {contact}");
                var mappedContact = _mapper.Map<Contact>(contact);
                var serviceResponse = _contactService.Create(mappedContact);
                var mappedResponseData = _mapper.Map<ExistingContactDTO>(serviceResponse.Data);
                return ApiResponseBuilder.ForCreate
                    .SetData(mappedResponseData)
                    .SetInformation(serviceResponse.Messages)
                    .Build();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, e.Message);
                return ApiResponseBuilder.ReturnFailure(_contextInteractor.GetTraceId(Request));
            }
        }

        [HttpGet("{id}")]
        [ProducesDefaultResponseType(typeof(ExistingContactDTO))]
        public IActionResult GetOne([FromRoute] int id)
        {
            try
            {
                _logger.LogTrace($"Entered {nameof(GetOne)} with arguments: {nameof(id)}: {id}");
                var response = _contactService.GetOne(id);
                var mappedContact = _mapper.Map<ExistingContactDTO>(response.Data);
                return ApiResponseBuilder.ForRead
                    .SetData(mappedContact)
                    .SetInformation(response.Messages)
                    .Build();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, e.Message);
                return ApiResponseBuilder.ReturnFailure(_contextInteractor.GetTraceId(Request));
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                _logger.LogTrace($"Entered {nameof(Delete)} with arguments: {nameof(id)}: {id}");
                var response = _contactService.Delete(id);
                return ApiResponseBuilder.ForDelete.SetInformation(response.Messages).Build();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, e.Message);
                return ApiResponseBuilder.ReturnFailure(_contextInteractor.GetTraceId(Request));
            }
        }

        [HttpPut("{id}")]
        [ProducesDefaultResponseType(typeof(ExistingContactDTO))]
        public IActionResult Update([FromRoute] int id, [FromBody] ContactDTO contact)
        {
            try
            {
                _logger.LogTrace($"Entered {nameof(Update)} with arguments: {nameof(id)}: {id}, {nameof(contact)}: {contact}");
                var mappedContact = _mapper.Map<Contact>(contact);
                mappedContact.Id = id;
                var response = _contactService.Update(mappedContact);
                var mappedResponse = _mapper.Map<ExistingContactDTO>(response.Data);
                return ApiResponseBuilder
                    .ForUpdate
                    .SetData(mappedResponse)
                    .SetInformation(response.Messages)
                    .Build();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, e.Message);
                return ApiResponseBuilder.ReturnFailure(_contextInteractor.GetTraceId(Request));
            }
        }

        [HttpGet("call-list")]
        [ProducesDefaultResponseType(typeof(IEnumerable<CallRecordDTO>))]
        public IActionResult GetCallList()
        {
            try
            {
                _logger.LogTrace($"Entered {nameof(GetCallList)}");
                var response = _contactService.GetCallList();
                var mappedResponseData = _mapper.Map<IEnumerable<CallRecordDTO>>(response.Data);
                return ApiResponseBuilder.ForRead.SetData(mappedResponseData).SetInformation(response.Messages).Build();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, e.Message);
                return ApiResponseBuilder.ReturnFailure(_contextInteractor.GetTraceId(Request));
            }
        }
    }
}
