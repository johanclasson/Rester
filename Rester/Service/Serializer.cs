using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json.Linq;
using Rester.Model;

namespace Rester.Service
{
    public interface ISerializer
    {
        Task<string> SerializeAsync(IEnumerable<ServiceConfiguration> configurations);
    }

    public interface IDeserializer
    {
        Task<ServiceConfiguration[]> DeserializeAsync(string data);
    }

    internal class Serializer : ISerializer
    {
        public Task<string> SerializeAsync(IEnumerable<ServiceConfiguration> configurations)
        {
            return Task.Run(() =>
            {
                var jConfigArray = new JArray();
                foreach (ServiceConfiguration configuration in configurations)
                {
                    JObject jConfig = SerializeConfiguration(configuration);
                    jConfigArray.Add(jConfig);
                }
                return new JObject
                {
                    ["Version"] = "0.1",
                    ["Configurations"] = jConfigArray
                }.ToString();
            });
        }

        private static JObject SerializeConfiguration(ServiceConfiguration configuration)
        {
            var jEndpointArray = new JArray();
            var jConfig = new JObject
            {
                ["Name"] = configuration.Name,
                ["BaseUri"] = configuration.BaseUri,
                ["Endpoints"] = jEndpointArray
            };
            foreach (ServiceEndpoint endpoint in configuration.Endpoints)
            {
                JObject jEndpoint = SerializeEndpoint(endpoint);
                jEndpointArray.Add(jEndpoint);
            }
            return jConfig;
        }

        private static JObject SerializeEndpoint(ServiceEndpoint endpoint)
        {
            var jActionArray = new JArray();
            var jEndpoint = new JObject
            {
                ["Name"] = endpoint.Name,
                ["Actions"] = jActionArray
            };
            foreach (ServiceEndpointAction action in endpoint.Actions)
            {
                JObject jAction = SerializeAction(action);
                jActionArray.Add(jAction);
            }
            return jEndpoint;
        }

        private static JObject SerializeAction(ServiceEndpointAction action)
        {
            var jAction = new JObject
            {
                ["Name"] = action.Name,
                ["Body"] = action.Body,
                ["MediaType"] = action.MediaType,
                ["Method"] = action.Method,
                ["UriPath"] = action.UriPath
            };
            return jAction;
        }
    }

    internal class Deserializer : IDeserializer
    {
        private readonly INavigationService _navigationService;
        private readonly IActionInvokerFactory _invokerFactory;

        public Deserializer(INavigationService navigationService, IActionInvokerFactory invokerFactory)
        {
            _navigationService = navigationService;
            _invokerFactory = invokerFactory;
        }

        public Task<ServiceConfiguration[]> DeserializeAsync(string data)
        {
            return Task.Run(() =>
            {
                JObject jData = JObject.Parse(data);
                return jData.GetJArray("Configurations").Select(CreateServiceConfiguration).ToArray();
            });
        }

        private ServiceConfiguration CreateServiceConfiguration(JObject jConfig)
        {
            var configuration = ServiceConfiguration.CreateSilently(
                jConfig.Get("Name"), jConfig.Get("BaseUri"),
                _navigationService, _invokerFactory);
            var endpoints = jConfig.GetJArray("Endpoints").Select(j => CreateEndpoint(j, configuration));
            configuration.Endpoints.AddRange(endpoints);
            return configuration;
        }

        private ServiceEndpoint CreateEndpoint(JObject jEndpoint, ServiceConfiguration configuration)
        {
            var endpoint = ServiceEndpoint.CreateSilently(jEndpoint.Get("Name"), configuration, _navigationService);
            endpoint.Actions.AddRange(jEndpoint.GetJArray("Actions").Select(j => CreateAction(j, configuration)));
            return endpoint;
        }

        private ServiceEndpointAction CreateAction(JObject jAction, ServiceConfiguration configuration)
        {
            return ServiceEndpointAction.CreateSilently(
                jAction.Get("Name"), jAction.Get("UriPath"),
                jAction.Get("Method"), jAction.Get("Body"),
                jAction.Get("MediaType"), () => configuration.BaseUri);
        }
    }

    internal static class JObjectExtensions
    {
        public static IEnumerable<JObject> GetJArray(this JObject jObject, string propertyName)
        {
            return jObject[propertyName].Cast<JObject>();
        }

        public static string Get(this JObject jObject, string propertyName)
        {
            return (string)jObject[propertyName];
        }
    }
}
