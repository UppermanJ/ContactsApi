﻿using System;
using System.Collections.Generic;
using System.Text;

namespace API.DTOs
{
    public class AddressDTO
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public override string ToString()
        {
            return $"{nameof(Street)}: {Street}, {nameof(City)}: {City}, {nameof(State)}: {State}, {nameof(Zip)}: {Zip}";
        }
    }
}
