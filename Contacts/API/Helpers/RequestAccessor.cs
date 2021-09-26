using API.Helpers.Interfaces;
using Microsoft.AspNetCore.Http;

namespace API.Helpers
{
    public class RequestAccessor: IRequestAccessor
    {
        public string GetTraceId(HttpRequest request) => request.HttpContext.TraceIdentifier;
    }
}
