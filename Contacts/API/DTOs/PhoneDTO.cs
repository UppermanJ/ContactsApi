using System.Linq.Expressions;
using Swashbuckle.AspNetCore.Annotations;

namespace API.DTOs
{
    [SwaggerSchema(Title = "Phone", Required = new []{ "type", "number"})]
    public class PhoneDTO
    {
        public string Number { get; set; }
        public PhoneTypeDTO Type { get; set; }

        public override string ToString()
        {
            return $"{nameof(Number)}: {Number}, {nameof(Type)}: {Type}";
        }
    }
}
