using API.DTOs;
using AutoMapper;
using Models;

namespace API.Mappers
{
    public class MapperConfiguration: Profile
    {
        public MapperConfiguration()
        {
            CreateMap<ContactDTO, Contact>();
            CreateMap<NameDTO, Name>();
            CreateMap<AddressDTO, Address>();
            CreateMap<PhoneDTO, Phone>();

            CreateMap<Contact, ExistingContactDTO>();
            CreateMap<Name, NameDTO>();
            CreateMap<Address, AddressDTO>();
            CreateMap<Phone, PhoneDTO>();
        }
    }
}
