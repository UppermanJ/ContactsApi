using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy.Generators;
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

        #region Update
        [Test]
        public void Update_WhenCalled_UpdatesContact()
        {
            var createdContact = _repository.Create(ContactTestHelpers.GenerateContact());
            var newContact = createdContact.Clone();
            newContact.Address.Zip += "different";
            
            var response = _repository.Update(newContact);

            Assert.That(response.Id, Is.EqualTo(newContact.Id));
            ContactTestHelpers.AssertContactMatchesAllButId(response, newContact);
        }
        
        [Test]
        public void Update_WhenCalledWithNonExistentContact_Idk()
        {
            var createdContact = _repository.Create(ContactTestHelpers.GenerateContact());
            var newContact = createdContact.Clone();
            newContact.Address.Zip += "different";
            newContact.Id += 1;
            var response = _repository.Update(newContact);

            Assert.That(response, Is.Null);
        }
        #endregion

        #region GetCallList

        [Test]
        public void GetCallList_WhenCalled_ReturnsProperObjects()
        {
            var userToCreateOne = ContactTestHelpers.GenerateContact();
            var userToCreateTwo = ContactTestHelpers.GenerateContact();
            var userToCreateThree = ContactTestHelpers.GenerateContact();

            _repository.Create(userToCreateOne);
            _repository.Create(userToCreateTwo);
            _repository.Create(userToCreateThree);

            var callList = _repository.GetCallList();

            Assert.That(callList.Count(), Is.EqualTo(3));
            ContactTestHelpers.AssertCallRecordMatchesContact(
                callList.FirstOrDefault(cl => cl.Name.First == userToCreateOne.Name.First), userToCreateOne);
            ContactTestHelpers.AssertCallRecordMatchesContact(
                callList.FirstOrDefault(cl => cl.Name.First == userToCreateTwo.Name.First), userToCreateTwo);
            ContactTestHelpers.AssertCallRecordMatchesContact(
                callList.FirstOrDefault(cl => cl.Name.First == userToCreateThree.Name.First), userToCreateThree);
        }

        [Test]
        public void GetCallList_WhenCalled_FiltersOutItemsWithoutHomePhone()
        {
            var userToCreateOne = ContactTestHelpers.GenerateContact();
            var userToCreateTwo = ContactTestHelpers.GenerateContact();
            var userToCreateThree = ContactTestHelpers.GenerateContact();
            userToCreateThree.Phone = userToCreateThree.Phone.Where(p => p.Type != PhoneType.Home).ToList();

            _repository.Create(userToCreateOne);
            _repository.Create(userToCreateTwo);
            _repository.Create(userToCreateThree);

            var callList = _repository.GetCallList();

            Assert.That(callList.Count(), Is.EqualTo(2));
            ContactTestHelpers.AssertCallRecordMatchesContact(
                callList.FirstOrDefault(cl => cl.Name.First == userToCreateOne.Name.First), userToCreateOne);
            ContactTestHelpers.AssertCallRecordMatchesContact(
                callList.FirstOrDefault(cl => cl.Name.First == userToCreateTwo.Name.First), userToCreateTwo);
        }

        [Test]
        public void GetCallList_WhenCalled_OrdersByLastName()
        {
            var shouldBeFirst = ContactTestHelpers.GenerateContact();
            shouldBeFirst.Name.Last = "A";
            var shouldBeSecond = ContactTestHelpers.GenerateContact();
            shouldBeSecond.Name.Last = "B";
            var shouldBeThird = ContactTestHelpers.GenerateContact();
            shouldBeThird.Name.Last = "C";


            _repository.Create(shouldBeThird);
            _repository.Create(shouldBeFirst);
            _repository.Create(shouldBeSecond);

            var callList = _repository.GetCallList();

            Assert.That(callList.Count(), Is.EqualTo(3));
            ContactTestHelpers.AssertCallRecordMatchesContact(
                callList.ToArray()[0], shouldBeFirst);
            ContactTestHelpers.AssertCallRecordMatchesContact(
                callList.ToArray()[1], shouldBeSecond);
            ContactTestHelpers.AssertCallRecordMatchesContact(
                callList.ToArray()[2], shouldBeThird);
        }

        [Test]
        public void GetCallList_WhenCalled_OrdersByLastNameThenFirstName()
        {
            var shouldBeFirst = ContactTestHelpers.GenerateContact();
            shouldBeFirst.Name.Last = "A";
            shouldBeFirst.Name.First = "Z";
            var shouldBeSecond = ContactTestHelpers.GenerateContact();
            shouldBeSecond.Name.Last = "C";
            shouldBeSecond.Name.First= "A";
            var shouldBeThird = ContactTestHelpers.GenerateContact();
            shouldBeThird.Name.Last = "C";
            shouldBeThird.Name.First = "B";


            _repository.Create(shouldBeThird);
            _repository.Create(shouldBeFirst);
            _repository.Create(shouldBeSecond);

            var callList = _repository.GetCallList();

            Assert.That(callList.Count(), Is.EqualTo(3));
            ContactTestHelpers.AssertCallRecordMatchesContact(
                callList.ToArray()[0], shouldBeFirst);
            ContactTestHelpers.AssertCallRecordMatchesContact(
                callList.ToArray()[1], shouldBeSecond);
            ContactTestHelpers.AssertCallRecordMatchesContact(
                callList.ToArray()[2], shouldBeThird);
        }
        
        [Test]
        public void GetCallList_WhenCalledContactHasNoMiddleName_ReturnsWithEmptyString()
        {
            var shouldBeFirst = ContactTestHelpers.GenerateContact();
            shouldBeFirst.Name.Middle = "";
            _repository.Create(shouldBeFirst);

            var callList = _repository.GetCallList();

            Assert.That(callList.First().Name.Middle, Is.EqualTo(""));
        }

        #endregion
    }
}
