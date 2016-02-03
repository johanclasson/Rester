using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json.Linq;
using Rester.Model;

namespace Rester.Service
{
    public interface ISerializer
    {
        string Serialize(ServiceConfiguration[] configurations);
    }

    public interface IDeserializer
    {
        ServiceConfiguration[] Deserialize(string data);
    }

    internal class Serializer : ISerializer
    {
        public string Serialize(ServiceConfiguration[] configurations)
        {
            var jConfigArray = new JArray();
            foreach (ServiceConfiguration configuration in configurations)
            {
                JObject jConfig = SerializeServiceConfiguraion(configuration);
                jConfigArray.Add(jConfig);
            }
            return jConfigArray.ToString();
        }

        private static JObject SerializeServiceConfiguraion(ServiceConfiguration configuration)
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

        public ServiceConfiguration[] Deserialize(string data)
        {
            JArray jArray = JArray.Parse(data);
            return jArray.Cast<JObject>().Select(CreateServiceConfiguration).ToArray();
        }

        private ServiceConfiguration CreateServiceConfiguration(JObject jConfig)
        {
            var configuration = new ServiceConfiguration(_navigationService, _invokerFactory)
            {
                Name = (string)jConfig["Name"],
                BaseUri = (string)jConfig["BaseUri"]
            };
            var endpoints = jConfig.GetJArray("Endpoints").Select(j => CreateEndpoint(j, configuration));
            configuration.Endpoints.AddRange(endpoints);
            return configuration;
        }

        private ServiceEndpoint CreateEndpoint(JObject jEndpoint, ServiceConfiguration configuration)
        {
            var endpoint = new ServiceEndpoint(configuration, _navigationService)
            {
                Name = (string)jEndpoint["Name"]
            };
            endpoint.Actions.AddRange(jEndpoint.GetJArray("Actions").Select(j => CreateAction(j, configuration)));
            return endpoint;
        }

        private ServiceEndpointAction CreateAction(JObject jAction, ServiceConfiguration configuration)
        {
            return new ServiceEndpointAction(() => configuration.BaseUri)
            {
                Name = (string)jAction["Name"],
                Body = (string)jAction["Body"],
                MediaType = (string)jAction["MediaType"],
                Method = (string)jAction["Method"],
                UriPath = (string)jAction["UriPath"]
            };
        }
    }

    internal static class JObjextExtensions
    {
        public static IEnumerable<JObject> GetJArray(this JObject jObject, string propertyName)
        {
            return jObject[propertyName].Cast<JObject>();
        }
    }
}
