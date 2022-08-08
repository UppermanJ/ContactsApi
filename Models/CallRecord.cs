namespace Models
{
    public class CallRecord
    {
        public Name Name { get; set; }
        public string Phone { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Phone)}: {Phone}";
        }
    }
}
