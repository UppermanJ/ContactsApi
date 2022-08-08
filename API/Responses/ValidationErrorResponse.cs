using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Models.ServiceLayerResponseWrapper;

namespace API.Responses
{
    public class ValidationErrorResponse
    {
        public string[] ValidationErrors { get; }

        public ValidationErrorResponse(IList<Information> messages)
        {
            ValidationErrors = messages.Select(m => m.Message).ToArray();
        }
    }
}
