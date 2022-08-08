namespace Models.ServiceLayerResponseWrapper
{
    public class Information
    {
        public string Message;

        public override string ToString()
        {
            return $"{nameof(Message)}: {Message}";
        }
    }

    public class NoActionNeeded : Information{}
    public class Error : Information { }
    public class ValidationError : Error { }
    public class NotFound : Error { }
}
