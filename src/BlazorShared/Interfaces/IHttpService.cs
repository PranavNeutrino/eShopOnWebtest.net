using System.Threading.Tasks;

namespace BlazorShared.Interfaces;

public interface IHttpService
{
    Task<T> HttpGet<T>(string uri) where T : class;
    Task<T> HttpDelete<T>(string uri, int id) where T : class;
    Task<T> HttpPost<T>(string uri, object dataToSend) where T : class;
    Task<T> HttpPut<T>(string uri, object dataToSend) where T : class;
}
