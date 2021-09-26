namespace Models.ServiceLayerResponseWrapper
{
    public class Information
    {
        public string Message;
    }

    public class Error : Information { }
    public class ValidationError : Error { }
    public class NotFound : Error { }
}
