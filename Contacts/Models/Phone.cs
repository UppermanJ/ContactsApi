using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Phone
    {
        public string Number { get; set; }
        public PhoneType Type { get; set; }

        public override string ToString()
        {
            return $"{nameof(Number)}: {Number}, {nameof(Type)}: {Type}";
        }
    }
}
