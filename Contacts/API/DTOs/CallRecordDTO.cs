using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace API.DTOs
{
    [SwaggerSchema(Title = "Call Record", Required = new []{"name", "phone"})]
    public class CallRecordDTO
    {
        public NameDTO Name { get; set; }
        public string Phone { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Phone)}: {Phone}";
        }
    }
}
