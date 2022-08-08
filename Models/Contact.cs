using System;
using System.Collections.Generic;
using System.Linq;

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
            return
                $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Address)}: {Address}, {nameof(Phone)}: {Phone}, {nameof(Email)}: {Email}";
        }

        public bool IsEqual(Contact contact)
        {
            return Id == contact.Id
                   && Email == contact.Email
                   && (Name?.IsEqual(contact.Name) ?? contact.Name == null)
                   && (Address?.IsEqual(contact.Address) ?? contact.Address == null)
                   && Phone.Count() == contact.Phone.Count()
                   && Phone.All(a => null != contact.Phone.FirstOrDefault(b => b.IsEqual(a)))
                   && contact.Phone.All(a => null != Phone.FirstOrDefault(b => b.IsEqual(a)));
        }

        public Contact Clone()
        {
            return new Contact()
            {
                Id = Id,
                Email = Email,
                Address = Address?.Clone(),
                Phone = Phone.Select(p => p.Clone()).ToList(),
                Name = Name?.Clone()
            };
        }

    }
}
