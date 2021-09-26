namespace Models.ServiceLayerResponseWrapper
{
    public class ServiceLayerResponseWrapper<T> : ServiceLayerResponse
    {
        public ServiceLayerResponseWrapper() { }

        public T Data { get; set; }
        public ServiceLayerResponseWrapper<T> SetData(T data)
        {
            Data = data;
            return this;
        }
        public new ServiceLayerResponseWrapper<T> AddInformation<I>(string message) where I : Information, new()
        {
            Messages.Add(new I() { Message = message });
            return this;
        }
    }
}
