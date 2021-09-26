using System.Collections.Generic;

using System.Linq;

namespace Models.ServiceLayerResponseWrapper
{
    public class ServiceLayerResponse<T>
    {
        public ServiceLayerResponse()
        {
            Messages = new List<Information>();
        }

        public bool HasErrors => Messages.Any(m => m is Error);
        public T Data { get; set; }
        public IList<Information> Messages { get; set; }
        public ServiceLayerResponse<T> SetData(T data)
        {
            Data = data;
            return this;
        }

        public ServiceLayerResponse<T> AddInformation<I>(string message) where I: Information, new()
        {
            Messages.Add(new I() { Message = message });
            return this;
        }
    }
}
