using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Rester.Model;

namespace Rester.Service
{
    internal interface IHttpClient
    {
        Task<HttpResponse> InvokeRestAction(ServiceEndpointAction action);
    }

    internal class HttpClientFactory : IHttpClient
    {
        private class HttpClientActionInvoker
        {
            private readonly ServiceEndpointAction _action;

            public HttpClientActionInvoker(ServiceEndpointAction action)
            {
                _action = action;
            }

            public async Task<HttpResponse> InvokeRestAction()
            {
                var watch = new Stopwatch();
                watch.Start();
                HttpResponseMessage result = await InvokeUriAsync();
                var content = await result.Content.ReadAsStringAsync();
                watch.Stop();
                var response = CreateHttpResponse(result, content, watch.Elapsed);
                return response;
            }

            private HttpResponse CreateHttpResponse(HttpResponseMessage result, string content, TimeSpan timeToResponse)
            {
                return new HttpResponse
                {
                    StatusCode = (int)result.StatusCode,
                    ReasonPhrase = result.ReasonPhrase,
                    Content = content.Trim(),
                    TimeToResponse = timeToResponse,
                    Uri = _action.Uri.AbsoluteUri,
                    Method = _action.Method,
                    CallTime = DateTime.Now,
                    IsSuccessfulStatusCode = result.IsSuccessStatusCode
                };
            }

            private async Task<HttpResponseMessage> InvokeUriAsync()
            {
                using (var client = new HttpClient())
                {
                    switch (_action.Method.ToLower())
                    {
                        case "get":
                            return await client.GetAsync(_action.Uri);
                        case "put":
                            return await client.PutAsync(_action.Uri, new StringContent(_action.Body, Encoding.UTF8, _action.MediaType));
                        case "post":
                            return await client.PostAsync(_action.Uri, new StringContent(_action.Body, Encoding.UTF8, _action.MediaType));
                        case "delete":
                            return await client.DeleteAsync(_action.Uri);
                        default:
                            throw new ArgumentException($"Encountered unknown http method {_action.Method}");
                    }
                }
            }
        }

        public Task<HttpResponse> InvokeRestAction(ServiceEndpointAction action)
        {
            var invoker = new HttpClientActionInvoker(action);
            return invoker.InvokeRestAction();
        }
    }
}