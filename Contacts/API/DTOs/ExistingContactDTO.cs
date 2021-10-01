using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace API.DTOs
{
    [SwaggerSchema(Title = "Existing Contact", Required = new []{"id", "name", "address", "phone", "email"})]
    public class ExistingContactDTO: ContactDTO
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Id)}: {Id}";
        }
    }
}
