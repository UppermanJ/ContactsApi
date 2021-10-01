using System;
using System.Collections.Generic;
using System.Text;
using Swashbuckle.AspNetCore.Annotations;

namespace API.DTOs
{
    [SwaggerSchema(Title = "PhoneType")]
    public enum PhoneTypeDTO
    {
        Home = 1,// to prevent defaulting,
        Work,
        Mobile
    }
}
