using API.DTOs;
using AutoMapper;
using Models;
using NUnit.Framework;
using CustomConfiguration = API.Mappers.MapperConfiguration;
using MapperConfiguration = AutoMapper.MapperConfiguration;

namespace API.Tests.Unit.Mappers
{
    public class MapperConfigurationTests
    {
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CustomConfiguration>());
            _mapper = config.CreateMapper();
        }

        [Test]
        public void Map_WhenMappingContactDtoToContact_PreventsNulls()
        {
            var emptyContact = new ContactDTO();
            var contact = _mapper.Map<Contact>(emptyContact);
            Assert.That(contact.Address.Zip, Is.EqualTo(""));
            Assert.That(contact.Address.City, Is.EqualTo(""));
            Assert.That(contact.Address.State, Is.EqualTo(""));
            Assert.That(contact.Address.Street, Is.EqualTo(""));
            Assert.That(contact.Email, Is.EqualTo(""));
            Assert.That(contact.Name.First, Is.EqualTo(""));
            Assert.That(contact.Name.Last, Is.EqualTo(""));
            Assert.That(contact.Name.Middle, Is.EqualTo(""));
            Assert.That(contact.Phone, Has.Count.EqualTo(0));
        }
    }
}
