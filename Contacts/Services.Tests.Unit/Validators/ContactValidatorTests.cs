using System.Collections.Generic;
using System.Linq;
using Models;
using NUnit.Framework;
using Services.Validators;

namespace Services.Tests.Unit.Validators
{
    class ContactValidatorTests
    {
        private ContactValidator _validator;
        private Contact _acceptableContact;

        [SetUp]
        public void Setup()
        {
            _acceptableContact = new Contact
            {
                Id = 0,
                Name = new Name
                {
                    First = "f-name",
                    Middle = null,
                    Last = "l-name"
                },
                Address = new Address
                {
                    Street = null,
                    City = null,
                    State = null,
                    Zip = null
                },
                Phone = new List<Phone>()
                {
                    new Phone
                    {
                        Number = "home-number",
                        Type = PhoneType.Home
                    },
                    new Phone
                    {
                        Number = "work-number",
                        Type = PhoneType.Work
                    },
                    new Phone
                    {
                        Number = "mobile-number",
                        Type = PhoneType.Mobile
                    }
                },
                Email = null
            };
            
            _validator = new ContactValidator();
        }

        [Test]
        public void Validate_WhenValid_ReturnsNoErrors()
        {
            var response = _validator.Validate(_acceptableContact);

            Assert.That(response.Errors, Has.Count.EqualTo(0));
        }
        
        [Test]
        public void Validate_WhenMoreThanOneHomePhone_ReturnsError()
        {
            _acceptableContact.Phone = _acceptableContact.Phone.Prepend(new Phone
            {
                Number = "Second-home-phone",
                Type = PhoneType.Home
            });
            var response = _validator.Validate(_acceptableContact);

            Assert.That(response.Errors, Has.Count.EqualTo(1));
            Assert.That(response.Errors.First(er => er.PropertyName == "Phone").ErrorMessage, Is.EqualTo("Only one phone number per type is allowed"));
        }

        [Test]
        public void Validate_WhenMoreThanOneMobilePhone_ReturnsError()
        {
            _acceptableContact.Phone = _acceptableContact.Phone.Prepend(new Phone
            {
                Number = "Second-mobile-phone",
                Type = PhoneType.Mobile
            });
            var response = _validator.Validate(_acceptableContact);

            Assert.That(response.Errors, Has.Count.EqualTo(1));
            Assert.That(response.Errors.First(er => er.PropertyName == "Phone").ErrorMessage, Is.EqualTo("Only one phone number per type is allowed"));
        }

        [Test]
        public void Validate_WhenMoreThanOneWorkPhone_ReturnsError()
        {
            _acceptableContact.Phone = _acceptableContact.Phone.Prepend(new Phone
            {
                Number = "Second-work-phone",
                Type = PhoneType.Work
            });
            var response = _validator.Validate(_acceptableContact);

            Assert.That(response.Errors, Has.Count.EqualTo(1));
            Assert.That(response.Errors.First(er => er.PropertyName == "Phone").ErrorMessage, Is.EqualTo("Only one phone number per type is allowed"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void Validate_WhenFirstNameIsNullOrEmpty_ReturnsError(string newFirstName)
        {
            _acceptableContact.Name.First = newFirstName;
            var response = _validator.Validate(_acceptableContact);

            Assert.That(response.Errors, Has.Count.EqualTo(1));
            Assert.That(response.Errors.First(er => er.PropertyName == "Name.First").ErrorMessage, Is.EqualTo("First Name is required"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void Validate_WhenLastNameIsNullOrEmpty_ReturnsError(string newLastName)
        {
            _acceptableContact.Name.Last= newLastName;
            var response = _validator.Validate(_acceptableContact);

            Assert.That(response.Errors, Has.Count.EqualTo(1));
            Assert.That(response.Errors.First(er => er.PropertyName == "Name.Last").ErrorMessage, Is.EqualTo("Last Name is required"));
        }
        
        [Test]
        public void Validate_WhenPhoneTypeIsNotValid_ReturnsError()
        {
            _acceptableContact.Phone = _acceptableContact.Phone.Prepend(new Phone
            {
                Number = "Second-work-phone",
                Type = (PhoneType)0
            });
            var response = _validator.Validate(_acceptableContact);

            Assert.That(response.Errors, Has.Count.EqualTo(1));
            Assert.That(response.Errors.First(er => er.PropertyName == "Phone[0]").ErrorMessage, Is.EqualTo("Phone Type must be either Home, Mobile or Work"));
        }[Test]
        public void Validate_WhenPhoneNumberIsEmpty_ReturnsError()
        {
            _acceptableContact.Phone.First().Number = "";
            var response = _validator.Validate(_acceptableContact);

            Assert.That(response.Errors, Has.Count.EqualTo(1));
            Assert.That(response.Errors.First(er => er.PropertyName == "Phone[0]").ErrorMessage, Is.EqualTo("Phone number is required"));
        }
    }
}
