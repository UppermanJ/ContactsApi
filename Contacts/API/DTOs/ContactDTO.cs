using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace API.DTOs
{
    [SwaggerSchema(Title = "Contact", Required = new []{"name"})]
    public class ContactDTO
    {
        public NameDTO Name { get; set; }
        public AddressDTO Address { get; set; }
        public IEnumerable<PhoneDTO> Phone { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Address)}: {Address}, {nameof(Phone)}: {Phone}, {nameof(Email)}: {Email}";
        }
    }
}
