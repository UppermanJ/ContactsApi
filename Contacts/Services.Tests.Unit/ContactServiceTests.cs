using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Repositories.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Models;
using Models.ServiceLayerResponseWrapper;
using Moq;
using NUnit.Framework;
using Test.Helpers;

namespace Services.Tests.Unit
{
    public class ContactServiceTests
    {
        private ContactService _service;
        private Mock<IContactRepository> _contactRepository;
        private Mock<ILogger<ContactService>> _logger;
        private Mock<IValidator<Contact>> _validatorMock;
        private Contact _simpleContact;
        private Contact _createdContact;
        private List<Contact> _simpleContactList;

        [SetUp]
        public void Setup()
        {
            _simpleContact = new Contact();
            _createdContact = new Contact();
            _simpleContactList = new List<Contact>();

            _validatorMock = new Mock<IValidator<Contact>>(MockBehavior.Strict);
            _validatorMock.Setup(vm => vm.Validate(It.IsAny<Contact>())).Returns(new ValidationResult());
            
            _contactRepository = new Mock<IContactRepository>(MockBehavior.Strict);
            _contactRepository.Setup(cr => cr.Create(It.IsAny<Contact>())).Returns(_createdContact);
            _contactRepository.Setup(cr => cr.GetAll()).Returns(_simpleContactList);
            _contactRepository.Setup(cr => cr.GetOne(It.IsAny<int>())).Returns(_simpleContact);
            _contactRepository.Setup(cr => cr.Delete(It.IsAny<int>()));


            _logger = new Mock<ILogger<ContactService>>();

            _service = new ContactService(_logger.Object, _contactRepository.Object, _validatorMock.Object);
        }

        [Test]
        public void Constructor_ShouldThrowErrorArgumentNullExceptions()
        {
            TestHelpers.AssertConstructorThrowsNullExceptionsWhenArgumentsAreNotProvided(typeof(ContactService));
        }

        #region Create

        [Test]
        public void Create_WhenCalled_CallsValidator()
        {
            _service.Create(_simpleContact);

            _validatorMock.Verify(vm => vm.Validate(_simpleContact));
        }

        [Test]
        public void Create_WhenValidatorReturnsErrors_ReturnsValidationErrors()
        {
            _validatorMock.Setup(vm => vm.Validate(_simpleContact)).Returns(new ValidationResult()
                {
                    Errors =
                    {
                        new ValidationFailure("prop1", "reason1"),
                        new ValidationFailure("prop2", "reason2")
                    }

                }
            );

            var response = _service.Create(_simpleContact);

            Assert.That(response.Messages, Has.Count.EqualTo(2));
            Assert.That(response.Messages.Select(m => m is ValidationError).ToList(), Has.Count.EqualTo(2));
            Assert.That(response.Messages.Select(m => m.Message), Does.Contain("prop1: reason1").And.Contains("prop2: reason2"));
        }

        [Test]
        public void Create_WhenValidatorReturnsNoErrors_CallsRepositoryCreateMethod()
        {
            _service.Create(_simpleContact);

            _contactRepository.Verify(cr => cr.Create(_simpleContact));
        }

        [Test]
        public void Create_WhenValidatorReturnsErrors_DoesNotCallRepository()
        {
            _validatorMock.Setup(vm => vm.Validate(_simpleContact)).Returns(new ValidationResult()
                {
                    Errors = { new ValidationFailure("", "") }
                }
            );

            _service.Create(_simpleContact);

            _contactRepository.Verify(cr => cr.Create(_simpleContact), Times.Never);
        }

        [Test]
        public void Create_WhenRepositoryCreateSucceeds_ReturnsCreatedContact()
        {
            var response =_service.Create(_simpleContact);

            Assert.That(response.Data, Is.EqualTo(_createdContact));
        }

        [Test]
        public void Create_WhenRepositoryThrows_BubblesException()
        {
            var exception = new Exception("Houston we had a problem");
            _contactRepository.Setup(cr => cr.Create(It.IsAny<Contact>())).Throws(exception);

            var thrownException = Assert.Throws<Exception>(() => _service.Create(_simpleContact));
            Assert.That(thrownException, Is.EqualTo(exception));
        }
        #endregion

        #region GetAll

        [Test]
        public void GetAll_WhenCalled_CallsRepositoryFunction()
        {
            _service.GetAll();

            _contactRepository.Verify(cr => cr.GetAll());
        }
        
        [Test]
        public void GetAll_WhenRepositoryReturnsValues_ReturnsThoseValuesInServiceLayerResponse()
        {
            var response = _service.GetAll();

            Assert.That(response.Data, Is.EqualTo(_simpleContactList));
        }

        [Test]
        public void GetAll_WhenRepositoryThrows_BubblesException()
        {
            var exception = new Exception("Houston we had a problem");
            _contactRepository.Setup(cr => cr.GetAll()).Throws(exception);

            var thrownException = Assert.Throws<Exception>(() => _service.GetAll());
            Assert.That(thrownException, Is.EqualTo(exception));
        }

        #endregion

        #region GetOne
        
        [Test]
        public void GetOne_WhenCalled_CallsRepository()
        {
            _service.GetOne(1);

            _contactRepository.Verify(cr => cr.GetOne(1));
        }

        [Test]
        public void GetOne_WhenRepositoryReturnsValue_ReturnsWrappedValue()
        {
            var response = _service.GetOne(1);

            Assert.That(response.Data, Is.EqualTo(_simpleContact));
        }

        [Test]
        public void GetOne_WhenRepositoryReturnsNull_ReturnsNotFoundMessage()
        {
            _contactRepository.Setup(cr => cr.GetOne(It.IsAny<int>())).Returns((Contact)null);
            
            var response = _service.GetOne(1);

            Assert.That(response.Messages, Has.Count.EqualTo(1));
            Assert.That(response.Messages.First(), Is.InstanceOf<NotFound>());
            Assert.That(response.Messages.First().Message, Is.EqualTo("No contact found with that Id"));
        }

        [Test]
        public void GetOne_WhenRepositoryThrows_ExceptionBubbles()
        {
            var exception = new Exception("Houston we had a problem");
            _contactRepository.Setup(cr => cr.GetOne(It.IsAny<int>())).Throws(exception);

            var thrownException = Assert.Throws<Exception>(() => _service.GetOne(1));
            Assert.That(thrownException, Is.EqualTo(exception));
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_WhenCalled_CallsRepositoryMethod()
        {
            _service.Delete(1);

            _contactRepository.Verify(cr => cr.Delete(1));
        }
        
        [Test]
        public void Delete_WhenRepositorySucceeds_ReturnsServiceLevelResponse()
        {
            var response = _service.Delete(1);

            Assert.That(response, Is.InstanceOf<ServiceLayerResponse>());
        }

        [Test] public void Delete_WhenRepositoryThrows_ExceptionBubbles()
        {
            var exception = new Exception("Houston we had a problem");
            _contactRepository.Setup(cr => cr.Delete(It.IsAny<int>())).Throws(exception);

            var thrownException = Assert.Throws<Exception>(() => _service.Delete(1));
            Assert.That(thrownException, Is.EqualTo(exception));
        }

        #endregion
    }
}
