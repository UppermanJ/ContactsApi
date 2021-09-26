using System.Collections.Generic;
using System.Linq;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ServiceLayerResponseWrapper;

namespace API.Helpers
{
    public class JsonResponseBuilder<T>
    {
        private readonly int _successStatusCode;
        private T _data { set; get; }
        private IList<Information> _information { set; get; }
        JsonResponseBuilder(int successStatusCode)
        {
            _successStatusCode = successStatusCode;
        }

        public JsonResponseBuilder<T> SetData(T data)
        {
            _data = data;
            return this;
        }

        public JsonResponseBuilder<T> SetInformation(IList<Information> information)
        {
            _information = information;
            return this;
        }

        public JsonResult Build()
        {
            if (_information.Any(i => i is ValidationError))
            {
                return new JsonResult(new ValidationErrorResponse(_information)) {StatusCode = 400};
            }
            return new JsonResult(_data) {StatusCode = _successStatusCode};
        }
        public static JsonResponseBuilder<T> ForCreate => new JsonResponseBuilder<T>(201);

        public static JsonResult ReturnFailure(string traceId) =>
            new JsonResult($"Something went wrong.  Log ID: {traceId}") {StatusCode = 500};
    }
}
