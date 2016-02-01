using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Rester.Model;

namespace Rester.Service
{
    internal interface IActionInvoker
    {
        Task<HttpResponse> InvokeRestAction();
    }

    internal interface IActionInvokerFactory
    {
        IActionInvoker CreateInvoker(string baseUri, ServiceEndpointAction action);
    }

    internal class ActionInvokerFactory : IActionInvokerFactory
    {
        public IActionInvoker CreateInvoker(string baseUri, ServiceEndpointAction action)
        {
            return new ActionInvokerActionInvoker(baseUri, action);
        }
    }

    internal class ActionInvokerActionInvoker : IActionInvoker
    {
        private readonly string _baseUri;
        private readonly ServiceEndpointAction _action;

        public ActionInvokerActionInvoker(string baseUri, ServiceEndpointAction action)
        {
            _baseUri = baseUri;
            _action = action;
        }

        private Uri Uri => new Uri(new Uri(_baseUri), _action.UriPath);

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
                Uri = Uri.AbsoluteUri,
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
                        return await client.GetAsync(Uri);
                    case "put":
                        return await client.PutAsync(Uri, new StringContent(_action.Body, Encoding.UTF8, _action.MediaType));
                    case "post":
                        return await client.PostAsync(Uri, new StringContent(_action.Body, Encoding.UTF8, _action.MediaType));
                    case "delete":
                        return await client.DeleteAsync(Uri);
                    default:
                        throw new ArgumentException($"Encountered unknown http method {_action.Method}");
                }
            }
        }
    }
}