using System.Collections.Generic;
using System.Linq;
using API.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ServiceLayerResponseWrapper;

namespace API.Helpers
{
    public class ApiResponseBuilder
    {
        protected readonly int _successStatusCode;
        private object _data { set; get; }
        protected IList<Information> _information { set; get; }
        protected ApiResponseBuilder(int successStatusCode)
        {
            _successStatusCode = successStatusCode;
        }
        
        public static ApiResponseBuilder ForDelete => new ApiResponseBuilder(StatusCodes.Status204NoContent);
        public static ApiResponseBuilder ForCreate => new ApiResponseBuilder(StatusCodes.Status201Created);
        public static ApiResponseBuilder ForRead => new ApiResponseBuilder(StatusCodes.Status200OK);
        public ApiResponseBuilder SetInformation(IList<Information> information)
        {
            _information = information;
            return this;
        }
        public ApiResponseBuilder SetData(object data)
        {
            _data = data;
            return this;
        }

        public IActionResult Build()
        {
            if (_information.Any(i => i is NotFound))
                return new NotFoundResult();
            if (_information.Any(i => i is ValidationError))
                return new JsonResult(new ValidationErrorResponse(_information)) { StatusCode = StatusCodes.Status400BadRequest };
            if (_successStatusCode == StatusCodes.Status204NoContent)
                return new NoContentResult();
            if (_data != null)
                return new JsonResult(_data) { StatusCode = _successStatusCode };

            return new StatusCodeResult(_successStatusCode);
        }

        public static JsonResult ReturnFailure(string traceId) =>
            new JsonResult($"Something went wrong.  Log ID: {traceId}") { StatusCode = 500 };
    }
}
