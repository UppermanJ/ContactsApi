using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using DataAccess.Repositories;
using DataAccess.Tests.Integration.Infrastructure;
using Models;
using NUnit.Framework;
using Test.Helpers;

namespace DataAccess.Tests.Integration.Repositories
{
    class ContactLiteDbRepositoryTests: LiteDbTestBase
    {
        private ContactLiteDbRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new ContactLiteDbRepository(_connection);
            _connection._db.Execute($"DELETE {_repository.CollectionName}");
        }

        [Test]
        public void Constructor_GivenNullArgument_ShouldThrowNullArgumentExceptions()
        {
            TestHelpers.AssertConstructorThrowsNullExceptionsWhenArgumentsAreNotProvided(typeof(ContactLiteDbRepository));
        }

        #region Create

        [Test]
        public void Create_WhenCalled_ReturnsCreatedUser()
        {
            var contactToCreate = GenerateContact();

            var createdContact = _repository.Create(contactToCreate);

            AssertContactMatchesAllButId(createdContact, contactToCreate);
        }

        [Test]
        public void Create_WhenCalled_ShouldAutoIncrementId()
        {
            var contactToCreate = GenerateContact();

            var createdContact = _repository.Create(contactToCreate);
            Assert.That(createdContact.Id, Is.Not.EqualTo(createdContact));
        }
        #endregion

        #region MyRegion
        private Contact GenerateContact()
        {
            return new Contact()
            {
                Id = 0,
                Name = new Name
                {
                    First = "name.first-" + Guid.NewGuid(),
                    Middle = "name.middle-" + Guid.NewGuid(),
                    Last = "name.last-" + Guid.NewGuid()
                },
                Address = new Address
                {
                    Street = "address.street-" + Guid.NewGuid(),
                    City = "address.city-" + Guid.NewGuid(),
                    State = "address.state-" + Guid.NewGuid(),
                    Zip = "address.zip-" + Guid.NewGuid()
                },
                Phone = new List<Phone>()
                {
                    new Phone
                    {
                        Number = "phone.mobile-" + Guid.NewGuid(),
                        Type = PhoneType.Mobile
                    },
                    new Phone
                    {
                        Number = "phone.work-" + Guid.NewGuid(),
                        Type = PhoneType.Work
                    }
                    ,new Phone
                    {
                        Number = "phone.home-" + Guid.NewGuid(),
                        Type = PhoneType.Home
                    }
                },
                Email = "email-" + Guid.NewGuid()
            };
        }

        private void AssertContactMatchesAllButId(Contact actual, Contact expected)
        {
            if (expected.Name == null)
            {
                Assert.That(actual.Name, Is.Null);
            }
            else
            {
                var actualName = actual.Name;
                var expectedName = expected.Name;
                Assert.That(actualName.First, Is.EqualTo(expectedName.First));
                Assert.That(actualName.Last, Is.EqualTo(expectedName.Last));
                Assert.That(actualName.Middle, Is.EqualTo(expectedName.Middle));
            }

            if (expected.Address == null)
            {
                Assert.That(actual.Address, Is.Null);
            }
            else
            {
                var actualAddress = actual.Address;
                var expectedAddress = expected.Address;
                Assert.That(actualAddress.City, Is.EqualTo(expectedAddress.City));
                Assert.That(actualAddress.State, Is.EqualTo(expectedAddress.State));
                Assert.That(actualAddress.Street, Is.EqualTo(expectedAddress.Street));
                Assert.That(actualAddress.Zip, Is.EqualTo(expectedAddress.Zip));
            }
            Assert.That(actual.Phone, Has.Count.EqualTo(expected.Phone.Count()));
            var index = 0;
            while (index < actual.Phone.Count())
            {
                var actualPhone = actual.Phone.ToArray()[index];
                var expectedPhone = expected.Phone.ToArray()[index];
                Assert.That(actualPhone.Number, Is.EqualTo(expectedPhone.Number));
                Assert.That(actualPhone.Type, Is.EqualTo(expectedPhone.Type));
                index++;
            }
            Assert.That(actual.Email, Is.EqualTo(expected.Email));
        }
        #endregion

    }
}
