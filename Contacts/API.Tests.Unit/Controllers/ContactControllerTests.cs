using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using API.Controllers;
using API.DTOs;
using API.Helpers.Interfaces;
using API.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Test.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.ServiceLayerResponseWrapper;
using Moq;
using Services.Interfaces;

namespace API.Tests.Unit.Controllers
{
    public class ContactControllerTests
    {
        private Mock<IContactService> _contactServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<ContactsController>> _loggerMock;
        private Mock<IRequestAccessor> _contextInteractor;
        private ContactsController _controller;
        private ContactDTO _simpleContactDto;
        private ExistingContactDTO _simpleExistingContactDto;
        private Contact _simpleMappedContact;
        private Contact _simpleReturnedContact;
        private IEnumerable<Contact> _simpleContactList;
        private List<ExistingContactDTO> _simpleExistingContactDTOList;
        private ServiceLayerResponseWrapper<Contact> _simpleServiceLayerContactResponse;
        private ServiceLayerResponseWrapper<IEnumerable<Contact>> _simpleServiceLayerIEnumerableContactResponse;
        private ServiceLayerResponse _simpleServiceLayerResponse;
        private string _traceId = "0404-0404-04040404-040404-0404";

        [SetUp]
        public void Setup()
        {
            _simpleContactDto = new ContactDTO();
            _simpleExistingContactDto = new ExistingContactDTO();
            _simpleMappedContact = new Contact();
            _simpleReturnedContact = new Contact();
            _simpleContactList = new List<Contact>();
            _simpleExistingContactDTOList = new List<ExistingContactDTO>();
            _simpleServiceLayerContactResponse = new ServiceLayerResponseWrapper<Contact>();
            _simpleServiceLayerIEnumerableContactResponse = new ServiceLayerResponseWrapper<IEnumerable<Contact>>();
            _simpleServiceLayerResponse = new ServiceLayerResponse();

            _contactServiceMock = new Mock<IContactService>(MockBehavior.Strict);
            _contactServiceMock.Setup(csm => csm.Create(It.IsAny<Contact>()))
                .Returns(_simpleServiceLayerContactResponse);
            _contactServiceMock.Setup(csm => csm.GetAll())
                .Returns(_simpleServiceLayerIEnumerableContactResponse);
            _contactServiceMock.Setup(csm => csm.GetOne(It.IsAny<int>())).Returns(_simpleServiceLayerContactResponse);
            _contactServiceMock.Setup(csm => csm.Delete(It.IsAny<int>())).Returns(_simpleServiceLayerResponse);

            _mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            _mapperMock.Setup(mm => mm.Map<Contact>(It.IsAny<ContactDTO>())).Returns(_simpleMappedContact);
            _mapperMock.Setup(mm => mm.Map<Contact>(It.Is<ContactDTO>(c => c == null))).Returns((Contact)null);
            _mapperMock.Setup(mm => mm.Map<ExistingContactDTO>(It.IsAny<Contact>())).Returns(_simpleExistingContactDto);
            _mapperMock.Setup(mm => mm.Map<ExistingContactDTO>(It.Is<Contact>(c => c == null))).Returns((ExistingContactDTO)null);
            _mapperMock.Setup(mm => mm.Map<IEnumerable<ExistingContactDTO>>(It.IsAny<IEnumerable<Contact>>()))
                .Returns(_simpleExistingContactDTOList);
            _mapperMock.Setup(mm => mm.Map<IEnumerable<ExistingContactDTO>>(It.Is<IEnumerable<Contact>>(r => r == null)))
                .Returns((IEnumerable<ExistingContactDTO>)null);

            _loggerMock = new Mock<ILogger<ContactsController>>();

            _contextInteractor = new Mock<IRequestAccessor>(MockBehavior.Strict);
            _contextInteractor.Setup(cim => cim.GetTraceId(It.IsAny<HttpRequest>())).Returns(_traceId);

            _controller = new ContactsController(_loggerMock.Object, _contactServiceMock.Object, _mapperMock.Object, _contextInteractor.Object);
        }

        [Test]
        public void Constructor_GivenNullArgument_ThrowsException()
        {
            TestHelpers.AssertConstructorThrowsNullExceptionsWhenArgumentsAreNotProvided(typeof(ContactsController));
        }
        
        #region Create
        [Test]
        public void Create_HasPostAttribute()
        {
            Assert.IsTrue(Attribute.IsDefined(typeof(ContactsController).GetMethod("Create"),
                typeof(HttpPostAttribute)));
        }

        [Test]
        public void Create_WhenCalled_CallsMapper()
        {
            _controller.Create(_simpleContactDto);

            _mapperMock.Verify(mm => mm.Map<Contact>(_simpleContactDto));
        }

        [Test]
        public void Create_WhenMapperReturnsSuccessfully_CallsCreateMethod()
        {
            _controller.Create(_simpleContactDto);

            _contactServiceMock.Verify(csm => csm.Create(_simpleMappedContact));
        }

        [Test]
        public void Create_WhenContactServiceReturnsResponse_MapsResponseDataToExistingContactDTO()
        {
            _simpleServiceLayerContactResponse.SetData(_simpleReturnedContact);

            _controller.Create(_simpleContactDto);

            _mapperMock.Verify(mm => mm.Map<ExistingContactDTO>(_simpleReturnedContact));
        }

        [Test]
        public void Create_WhenMapperMapsResponseAppropriately_ReturnsMappedValue()
        {
            _simpleServiceLayerContactResponse.SetData(_simpleReturnedContact);

            var response = _controller.Create(_simpleContactDto) as JsonResult;

            Assert.That(response.Value.Equals(_simpleExistingContactDto));
        }

        [Test]
        public void Create_WhenMapperMapsResponseAppropriately_ReturnsCorrectStatusCode()
        {
            _simpleServiceLayerContactResponse.SetData(_simpleReturnedContact);

            var response = _controller.Create(_simpleContactDto) as JsonResult;

            Assert.That(response.StatusCode, Is.EqualTo(201));
        }

        [Test]
        public void Create_WhenContactServiceResponseThrows_ReturnsErrorMessageWithLoggingData()
        {
            _contactServiceMock.Setup(csm => csm.Create(It.IsAny<Contact>())).Throws(new Exception("It failed"));

            var response = _controller.Create(_simpleContactDto) as JsonResult;

            Assert.That(response.Value, Is.EqualTo(TestHelpers.GetDefault500Message(_traceId)));
        }

        [Test]
        public void Create_WhenContactServiceResponseThrows_Returns500StatusCode()
        {
            _contactServiceMock.Setup(csm => csm.Create(It.IsAny<Contact>())).Throws(new Exception("It failed"));

            var response = _controller.Create(_simpleContactDto) as JsonResult;

            Assert.That(response.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void Create_WhenContactServiceResponseThrows_LogsException()
        {
            var simpleException = new Exception("It failed");
            _contactServiceMock.Setup(csm => csm.Create(It.IsAny<Contact>())).Throws(simpleException);

            _controller.Create(_simpleContactDto);

            _loggerMock.VerifyCriticalWasCalled(simpleException.Message);
        }

        [Test]
        public void Create_WhenContactServiceResponseReturnsValidationErrors_ReturnsValidationErrors()
        {
            var validationErrorOne = "Email is invalid";
            var validationErrorTwo = "Only one home phone number is allowed";
            _simpleServiceLayerContactResponse
                .AddInformation<ValidationError>(validationErrorOne)
                .AddInformation<ValidationError>(validationErrorTwo);

            var response = _controller.Create(_simpleContactDto) as JsonResult;

            Assert.That(((ValidationErrorResponse) response.Value).ValidationErrors,
                Has.Length.EqualTo(2)
                    .And.Contains(validationErrorOne)
                    .And.Contains(validationErrorTwo)
                );
        }

        [Test]
        public void Create_WhenContactServiceResponseReturnsValidationErrors_Returns400StatusCode()
        {
            var validationErrorOne = "Email is invalid";
            var validationErrorTwo = "Only one home phone number is allowed";
            _simpleServiceLayerContactResponse
                .AddInformation<ValidationError>(validationErrorOne)
                .AddInformation<ValidationError>(validationErrorTwo);

            var response = _controller.Create(_simpleContactDto) as JsonResult;

            Assert.That(response.StatusCode, Is.EqualTo(400));
        }
        #endregion

        #region GetAll
        [Test]
        public void GetAll_HasGetAttribute()
        {
            Assert.IsTrue(Attribute.IsDefined(typeof(ContactsController).GetMethod("GetAll"),
                typeof(HttpGetAttribute)));
        }

        [Test]
        public void GetAll_WhenCalled_CallsServiceLayerGetAllFunction()
        {
            _controller.GetAll();

            _contactServiceMock.Verify(csm => csm.GetAll());
        }
        
        [Test]
        public void GetAll_ServiceLayerReturnsValues_MapsAllValues()
        {
            _simpleServiceLayerIEnumerableContactResponse.SetData(_simpleContactList);

            _controller.GetAll();

            _mapperMock.Verify(mm => mm.Map<IEnumerable<ExistingContactDTO>>(_simpleContactList));
        }

        [Test]
        public void GetAll_WhenMapperReturnsObjects_ReturnsJsonWithThoseValues()
        {
            var contactDtoOne = new ExistingContactDTO();
            var contactDtoTwo = new ExistingContactDTO();
            _simpleExistingContactDTOList.Add(contactDtoOne);
            _simpleExistingContactDTOList.Add(contactDtoTwo);
            _simpleServiceLayerIEnumerableContactResponse.SetData(_simpleContactList);

            var response = _controller.GetAll();

            Assert.That(((JsonResult) response).Value,
                Has.Count.EqualTo(2).And.Contains(contactDtoOne).And.Contains(contactDtoTwo));
        }

        [Test]
        public void GetAll_WhenMapperReturnsObjects_Returns200StatusCode()
        {
            var contactDtoOne = new ExistingContactDTO();
            var contactDtoTwo = new ExistingContactDTO();
            _simpleExistingContactDTOList.Add(contactDtoOne);
            _simpleExistingContactDTOList.Add(contactDtoTwo);
            _simpleServiceLayerIEnumerableContactResponse.SetData(_simpleContactList);

            var response = _controller.GetAll();

            Assert.That(((JsonResult) response).StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void GetAll_WhenServiceLayerThrows_ReturnsErrorMessage()
        {
            _contactServiceMock.Setup(csm => csm.GetAll()).Throws(new Exception());

            var response = _controller.GetAll() as JsonResult;

            Assert.That(response.Value, Is.EqualTo(TestHelpers.GetDefault500Message(_traceId)));
        }

        [Test]
        public void GetAll_WhenServiceLayerThrows_Returns500StatusCode()
        {
            _contactServiceMock.Setup(csm => csm.GetAll()).Throws(new Exception());

            var response = _controller.GetAll() as JsonResult;

            Assert.That(response.StatusCode, Is.EqualTo(500));
        }
        
        [Test]
        public void GetAll_WhenServiceLayerThrows_LogsCriticalError()
        {
            var exception = new Exception("My String");
            _contactServiceMock.Setup(csm => csm.GetAll()).Throws(exception);

            _controller.GetAll();

            _loggerMock.VerifyCriticalWasCalled(exception.Message);
        }
        #endregion

        #region GetOne

        [Test]
        public void GetOne_HasGetAttribute()
        {
            Assert.IsTrue(Attribute.IsDefined(typeof(ContactsController).GetMethod("GetOne"), typeof(HttpGetAttribute)));
        }

        [Test]
        public void GetOne_WhenCalled_CallsServiceLayerMethod()
        {
            _controller.GetOne(1);

            _contactServiceMock.Verify(csm => csm.GetOne(1));
        }

        [Test]
        public void GetOne_WhenServiceLayerReturns_MapsReturnedObject()
        {
            _simpleServiceLayerContactResponse.SetData(_simpleReturnedContact);
            
            _controller.GetOne(1);

            _mapperMock.Verify(mm => mm.Map<ExistingContactDTO>(_simpleReturnedContact));
        }

        [Test]
        public void GetOne_WhenMapperReturns_ReturnsMappedObject()
        {
            _simpleServiceLayerContactResponse.SetData(_simpleReturnedContact);

            var response = _controller.GetOne(1) as JsonResult;

            Assert.That(response.Value, Is.EqualTo(_simpleExistingContactDto));
        }
        
        [Test]
        public void GetOne_WhenMapperReturns_Returns200StatusCode()
        {
            _simpleServiceLayerContactResponse.SetData(_simpleReturnedContact);

            var response = _controller.GetOne(1) as JsonResult;

            Assert.That(response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void GetOne_WhenServiceReturnsNotFoundMessage_Returns404StatusCode()
        {
            _simpleServiceLayerContactResponse.AddInformation<NotFound>("Not Found");

            var response = _controller.GetOne(1) as StatusCodeResult;

            Assert.That(response.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public void GetOne_WhenServiceThrowsException_ReturnsErrorMessage()
        {
            _contactServiceMock.Setup(csm => csm.GetOne(It.IsAny<int>())).Throws(new Exception());

            var response = _controller.GetOne(1) as JsonResult;

            Assert.That(response.Value, Is.EqualTo(TestHelpers.GetDefault500Message(_traceId)));
        }

        [Test]
        public void GetOne_WhenServiceThrowsException_Returns500StatusCode()
        {
            _contactServiceMock.Setup(csm => csm.GetOne(It.IsAny<int>())).Throws(new Exception());

            var response = _controller.GetOne(1) as JsonResult;

            Assert.That(response.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void GetOne_WhenServiceThrowsException_LogsException()
        {
            var exception = new Exception("Houston we have a problem");
            _contactServiceMock.Setup(csm => csm.GetOne(It.IsAny<int>())).Throws(exception);

            _controller.GetOne(1);

            _loggerMock.VerifyCriticalWasCalled(exception.Message);
        }
        #endregion

        #region Delete

        [Test] public void Delete_HasDeleteAttribute()
        {
            Assert.IsTrue(Attribute.IsDefined(typeof(ContactsController).GetMethod("Delete"), typeof(HttpDeleteAttribute)));
        }

        [Test]
        public void Delete_WhenCalled_CallsInteractorMethod()
        {
            _controller.Delete(1);

            _contactServiceMock.Verify(csm => csm.Delete(1));
        }
        
        [Test]
        public void Delete_WhenServiceLayerResponds_Returns204()
        {
            var result = _controller.Delete(1) as StatusCodeResult;

            Assert.That(result.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public void Delete_WhenServiceLayerThrows_Returns500Message()
        {
            _contactServiceMock.Setup(csm => csm.Delete(It.IsAny<int>())).Throws(new Exception());

            var result = _controller.Delete(1) as JsonResult;

            Assert.That(result.Value, Is.EqualTo(TestHelpers.GetDefault500Message(_traceId)));
        }
        [Test]
        public void Delete_WhenServiceLayerThrows_Returns500StatusCode()
        {
            _contactServiceMock.Setup(csm => csm.Delete(It.IsAny<int>())).Throws(new Exception());

            var result = _controller.Delete(1) as JsonResult;

            Assert.That(result.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void Delete_WhenServiceThrowsException_LogsException()
        {
            var exception = new Exception("Houston we have a problem");
            _contactServiceMock.Setup(csm => csm.Delete(It.IsAny<int>())).Throws(exception);

            _controller.Delete(1);

            _loggerMock.VerifyCriticalWasCalled(exception.Message);
        }
        #endregion

        #region Update
        [Test]
        public void Update_WhenServiceThrowsException_LogsException()
        {
            Assert.IsTrue(Attribute.IsDefined(typeof(ContactsController).GetMethod("Update"), typeof(HttpPutAttribute)));
        }
        
        [Test]
        public void Update_WhenCalled_MapsGivenContractDto()
        {
            _controller.Update(1, _simpleContactDto);

            _mapperMock.Verify(mm => mm.Map<Contact>(_simpleContactDto));
        }
        #endregion
    }
}

