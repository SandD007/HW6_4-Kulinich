using MVC.ViewModels;

namespace MVC.Services.Interfaces;

public interface IHttpClientService
{
    Task<TResponse> SendAsync<TResponse, TRequest>(string url, HttpMethod method, TRequest? content);

    Task<TResponse> GetAsync<TResponse>(string url, HttpMethod method);
}