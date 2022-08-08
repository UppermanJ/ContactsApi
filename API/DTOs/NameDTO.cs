using System;
using System.Collections.Generic;
using System.Text;

namespace API.DTOs
{
    public class NameDTO
    {
        public string First { get; set; }
        public string Middle { get; set; }
        public string Last { get; set; }

        public override string ToString()
        {
            return $"{nameof(First)}: {First}, {nameof(Middle)}: {Middle}, {nameof(Last)}: {Last}";
        }
    }
}
