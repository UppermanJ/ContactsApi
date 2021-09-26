namespace API.DTOs
{
    public class PhoneDTO
    {
        public string Number { get; set; }
        public PhoneTypeDTO Type { get; set; }

        public override string ToString()
        {
            return $"{nameof(Number)}: {Number}, {nameof(Type)}: {Type}";
        }
    }
}
