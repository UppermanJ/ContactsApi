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
            var contactToCreate = ContactTestHelpers.GenerateContact();

            var createdContact = _repository.Create(contactToCreate);

            ContactTestHelpers.AssertContactMatchesAllButId(createdContact, contactToCreate);
        }

        [Test]
        public void Create_WhenCalled_ShouldAutoIncrementId()
        {
            var contactToCreate = ContactTestHelpers.GenerateContact();

            var createdContact = _repository.Create(contactToCreate);
            Assert.That(createdContact.Id, Is.Not.EqualTo(createdContact));
        }
        #endregion

        #region GetAll

        [Test]
        public void GetAll_WhenCalledWithExistingContacts_ReturnsAllContacts()
        {
            var createdContactOne = _repository.Create(ContactTestHelpers.GenerateContact());
            var createdContactTwo = _repository.Create(ContactTestHelpers.GenerateContact());

            var foundContacts = _repository.GetAll();

            ContactTestHelpers.AssertAllContactsMatch(foundContacts, new List<Contact>() {createdContactOne, createdContactTwo});
        }
        [Test]
        public void GetAll_WhenCalledWithNoContacts_ReturnsEmptyEnumerable()
        {
            var foundContacts = _repository.GetAll();

            Assert.That(foundContacts, Is.InstanceOf<IEnumerable<Contact>>().And.Empty);
        }

        #endregion

        #region GetOne

        [Test]
        public void GetOne_WhenCalledWithExistingContact_ReturnsContact()
        {
            var createdContact = _repository.Create(ContactTestHelpers.GenerateContact());

            var foundContact = _repository.GetOne(createdContact.Id);

            Assert.That(foundContact.Id, Is.EqualTo(createdContact.Id));
            ContactTestHelpers.AssertContactMatchesAllButId(foundContact, createdContact);
        }

        [Test]
        public void GetOne_WhenCalledWithNonExistingExistingContactId_ReturnsNull()
        {
            var createdContact = _repository.Create(ContactTestHelpers.GenerateContact());

            var foundContact = _repository.GetOne(createdContact.Id + 1);

            Assert.That(foundContact, Is.Null);
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_WhenCalledWithExistingId_DeletesItem()
        {
            var createdContact = _repository.Create(ContactTestHelpers.GenerateContact());

            _repository.Delete(createdContact.Id);

            var foundContact = _repository.GetOne(createdContact.Id);
            Assert.That(foundContact, Is.Null);
        }

        [Test]
        public void Delete_WhenCalledWithNonExisting_DoesNotThrow()
        {
            var createdContact = _repository.Create(ContactTestHelpers.GenerateContact());

            _repository.Delete(createdContact.Id + 1);
        }

        #endregion

    }
}
