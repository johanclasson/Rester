using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Rester.Model;

namespace Rester.Service
{
    public interface IActionInvoker
    {
        Task<HttpResponse> InvokeRestActionAsync();
    }

    public interface IActionInvokerFactory
    {
        IActionInvoker CreateInvoker(string baseUri, ServiceAction action);
    }

    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated through IoC
    internal class ActionInvokerFactory : IActionInvokerFactory
    {
        public IActionInvoker CreateInvoker(string baseUri, ServiceAction action)
        {
            return new ActionInvokerActionInvoker(baseUri, action);
        }
    }

    internal class ActionInvokerActionInvoker : IActionInvoker
    {
        private readonly string _baseUri;
        private readonly ServiceAction _action;

        public ActionInvokerActionInvoker(string baseUri, ServiceAction action)
        {
            _baseUri = baseUri;
            _action = action;
        }

        private Uri Uri => new Uri(new Uri(_baseUri), _action.UriPath);

        public async Task<HttpResponse> InvokeRestActionAsync()
        {
            // Not sure why this extra layer of async task is needed, but it is!
            return await Task.Run(async () =>
            {
                var callTime = DateTime.Now;
                var watch = new Stopwatch();
                watch.Start();
                try
                {
                    HttpResponseMessage result = await InvokeUriAsync();
                    var content = await result.Content.ReadAsStringAsync();
                    watch.Stop();
                    return CreateHttpResponse((int)result.StatusCode, result.IsSuccessStatusCode, result.ReasonPhrase, content.Trim(), watch, callTime);
                }
                catch (HttpRequestException ex)
                {
                    watch.Stop();
                    return CreateHttpResponse(null, false, ex.Message, string.Empty, watch, callTime);
                }
            });
        }

        private HttpResponse CreateHttpResponse(int? statusCode, bool isSuccessfulStatusCode, string reasonPhrase, string content, Stopwatch watch, DateTime callTime)
        {
            return new HttpResponse
            {
                StatusCode = statusCode,
                ReasonPhrase = reasonPhrase,
                Content = content,
                TimeToResponse = watch.Elapsed,
                Uri = Uri.AbsoluteUri,
                Method = _action.Method,
                CallTime = callTime,
                IsSuccessfulStatusCode = isSuccessfulStatusCode
            };
        }

        private async Task<HttpResponseMessage> InvokeUriAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
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