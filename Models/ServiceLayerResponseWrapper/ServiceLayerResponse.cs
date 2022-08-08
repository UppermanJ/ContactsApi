using System;
using System.Collections.Generic;
using System.Linq;

namespace Models.ServiceLayerResponseWrapper
{
    public class ServiceLayerResponse
    {
        public IList<Information> Messages { get; set; }
        public bool HasErrors => Messages.Any(m => m is Error);
        public ServiceLayerResponse()
        {
            Messages = new List<Information>();
        }
        public ServiceLayerResponse AddInformation<I>(string message) where I : Information, new()
        {
            Messages.Add(new I() { Message = message });
            return this;
        }

        public override string ToString()
        {
            return $"{nameof(Messages)}: {String.Join(",", Messages.Select(m => m.ToString()))}, {nameof(HasErrors)}: {HasErrors}";
        }
    }
}