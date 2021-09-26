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

        public bool IsEqual(Phone phone)
        {
            return Number == phone.Number && Type == phone.Type;
        }

        public Phone Clone()
        {
            return new Phone()
            {
                Number = Number,
                Type = Type
            };
        }
    }
}
