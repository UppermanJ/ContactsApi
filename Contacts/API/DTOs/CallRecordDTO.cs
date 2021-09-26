namespace API.DTOs
{
    public class CallRecordDTO
    {
        public NameDTO Name { get; set; }
        public string Phone { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Phone)}: {Phone}";
        }
    }
}
