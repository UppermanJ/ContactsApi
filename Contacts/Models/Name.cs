using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Name
    {
        public string First { get; set; }
        public string Middle { get; set; }
        public string Last { get; set; }

        public override string ToString()
        {
            return $"{nameof(First)}: {First}, {nameof(Middle)}: {Middle}, {nameof(Last)}: {Last}";
        }

        public bool IsEqual(Name name)
        {
            return First == name.First
                && Middle == name.Middle
                && Last == name.Last;
        }

        public Name Clone()
        {
            return new Name()
            {
                First = First,
                Middle = Middle,
                Last = Last
            };
        }
    }
}
