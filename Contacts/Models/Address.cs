using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace Models
{
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public override string ToString()
        {
            return $"{nameof(Street)}: {Street}, {nameof(City)}: {City}, {nameof(State)}: {State}, {nameof(Zip)}: {Zip}";
        }

        public bool IsEqual(Address address)
        {
            return Street == address.Street 
                   && City == address.City 
                   && State == address.State 
                   && Zip == address.Zip;
        }

        public Address Clone()
        {
            return new Address()
            {
                State = State,
                City = City,
                Street = Street,
                Zip = Zip
            };
        }
    }
}
