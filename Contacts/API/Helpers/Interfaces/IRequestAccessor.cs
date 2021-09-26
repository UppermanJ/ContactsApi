using Microsoft.AspNetCore.Http;

namespace API.Helpers.Interfaces
{
    public interface IRequestAccessor
    {
        string GetTraceId(HttpRequest request);
    }
}