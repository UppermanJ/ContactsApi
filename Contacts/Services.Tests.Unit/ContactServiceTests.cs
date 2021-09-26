using System;
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

        [SetUp]
        public void Setup()
        {
            _simpleContact = new Contact();
            _createdContact = new Contact();
            
            _validatorMock = new Mock<IValidator<Contact>>(MockBehavior.Strict);
            _validatorMock.Setup(vm => vm.Validate(It.IsAny<Contact>())).Returns(new ValidationResult());
            
            _contactRepository = new Mock<IContactRepository>(MockBehavior.Strict);
            _contactRepository.Setup(cr => cr.Create(It.IsAny<Contact>())).Returns(_createdContact);

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
        public void Create_WhenRepositoryThroes_BubblesException()
        {
            var exception = new Exception("Houston we had a problem");
            _contactRepository.Setup(cr => cr.Create(It.IsAny<Contact>())).Throws(exception);

            var thrownException = Assert.Throws<Exception>(() => _service.Create(_simpleContact));
            Assert.That(thrownException, Is.EqualTo(exception));
        }
        #endregion
    }
}
