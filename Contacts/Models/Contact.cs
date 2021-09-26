using System;
using System.Collections.Generic;

namespace Models
{
    public class Contact
    {
        public int Id { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }
        public IEnumerable<Phone> Phone { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Address)}: {Address}, {nameof(Phone)}: {Phone}, {nameof(Email)}: {Email}";
        }
    }
}
