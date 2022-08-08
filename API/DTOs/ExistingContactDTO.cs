using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ExistingContactDTO: ContactDTO
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Id)}: {Id}";
        }
    }
}
