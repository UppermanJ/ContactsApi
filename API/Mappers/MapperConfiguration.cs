using System.Collections.Generic;
using API.DTOs;
using AutoMapper;
using Models;

namespace API.Mappers
{
    public class MapperConfiguration : Profile
    {
        public MapperConfiguration()
        {
            CreateMap<ContactDTO, Contact>()
                .ForMember(
                    dest => dest.Phone,
                    opt => opt.MapFrom(
                        src => src.Phone ?? new List<PhoneDTO>()
                    ))
                .ForMember(
                    dest => dest.Address,
                    opt => opt.MapFrom(
                        src => src.Address ?? new AddressDTO
                            {Street = "", City = "", State = "", Zip = ""}))
                .ForMember(
                    dest => dest.Name, 
                    opt => opt.MapFrom(
                        src => src.Name ?? new NameDTO 
                            {First = "", Middle = "", Last = ""}))
                .ForMember(
                    dest => dest.Email, 
                    opt => opt.MapFrom(
                        src => src.Email ?? ""));
            CreateMap<NameDTO, Name>()
                .ForMember(dest => dest.Middle, opt => opt.DoNotAllowNull())
                .ForMember(dest => dest.Last, opt => opt.DoNotAllowNull())
                .ForMember(dest => dest.First, opt => opt.DoNotAllowNull());
            CreateMap<AddressDTO, Address>()
                .ForMember(dest => dest.Street, opt => opt.DoNotAllowNull())
                .ForMember(dest => dest.State, opt => opt.DoNotAllowNull())
                .ForMember(dest => dest.City, opt => opt.DoNotAllowNull())
                .ForMember(dest => dest.Zip, opt => opt.DoNotAllowNull());
            CreateMap<PhoneDTO, Phone>()
                .ForMember(dest => dest.Number, opt => opt.DoNotAllowNull());

            CreateMap<Contact, ExistingContactDTO>();
            CreateMap<Name, NameDTO>();
            CreateMap<Address, AddressDTO>();
            CreateMap<Phone, PhoneDTO>();

            CreateMap<CallRecord, CallRecordDTO>();
        }
    }
}
