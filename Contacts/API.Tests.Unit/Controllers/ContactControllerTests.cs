using System;
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
        private Mock<ILogger<ContactController>> _loggerMock;
        private Mock<IRequestAccessor> _contextInteractor;
        private ContactController _controller;
        private ContactDTO _simpleContactDto;
        private ExistingContactDTO _simpleExistingContactDto;
        private Contact _simpleMappedContact;
        private Contact _simpleReturnedContact;
        private ServiceLayerResponse<Contact> _simpleServiceLayerContactResponse;
        private string _traceId = "0404-0404-04040404-040404-0404";

        [SetUp]
        public void Setup()
        {
            _simpleContactDto = new ContactDTO();
            _simpleExistingContactDto = new ExistingContactDTO();
            _simpleMappedContact = new Contact();
            _simpleReturnedContact = new Contact();
            _simpleServiceLayerContactResponse = new ServiceLayerResponse<Contact>();

            _contactServiceMock = new Mock<IContactService>(MockBehavior.Strict);
            _contactServiceMock.Setup(csm => csm.Create(It.IsAny<Contact>()))
                .Returns(_simpleServiceLayerContactResponse);

            _mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            _mapperMock.Setup(mm => mm.Map<Contact>(It.IsAny<ContactDTO>())).Returns(_simpleMappedContact);
            _mapperMock.Setup(mm => mm.Map<ExistingContactDTO>(It.IsAny<Contact>())).Returns(_simpleExistingContactDto);

            _loggerMock = new Mock<ILogger<ContactController>>();

            _contextInteractor = new Mock<IRequestAccessor>(MockBehavior.Strict);
            _contextInteractor.Setup(cim => cim.GetTraceId(It.IsAny<HttpRequest>())).Returns(_traceId);

            _controller = new ContactController(_loggerMock.Object, _contactServiceMock.Object, _mapperMock.Object, _contextInteractor.Object);
        }

        [Test]
        public void Constructor_GivenNullArgument_ThrowsException()
        {
            TestHelpers.AssertConstructorThrowsNullExceptionsWhenArgumentsAreNotProvided(typeof(ContactController));
        }
        
        #region Create
        [Test]
        public void Create_HasPostAttribute()
        {
            Assert.IsTrue(Attribute.IsDefined(typeof(ContactController).GetMethod("Create"),
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

            Assert.That(response.Value, Is.EqualTo($"Something went wrong.  Log ID: {_traceId}"));
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
    }
}

