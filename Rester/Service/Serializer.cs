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
            var jActionGroupArray = new JArray();
            var jConfig = new JObject
            {
                ["Name"] = configuration.Name,
                ["BaseUri"] = configuration.BaseUri,
                ["ActionGroups"] = jActionGroupArray
            };
            foreach (ActionGroup actionGroup in configuration.ActionGroups)
            {
                JObject jActionGroup = SerializeActionGroup(actionGroup);
                jActionGroupArray.Add(jActionGroup);
            }
            return jConfig;
        }

        private static JObject SerializeActionGroup(ActionGroup actionGroup)
        {
            var jActionArray = new JArray();
            var jActionGroup = new JObject
            {
                ["Name"] = actionGroup.Name,
                ["Actions"] = jActionArray
            };
            foreach (ServiceAction action in actionGroup.Actions)
            {
                JObject jAction = SerializeAction(action);
                jActionArray.Add(jAction);
            }
            return jActionGroup;
        }

        private static JObject SerializeAction(ServiceAction action)
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
            var actionGroups = jConfig.GetJArray("ActionGroups").Select(j => CreateActionGroup(j, configuration));
            configuration.ActionGroups.AddRange(actionGroups);
            return configuration;
        }

        private ActionGroup CreateActionGroup(JObject jActionGroup, ServiceConfiguration configuration)
        {
            var actionGroup = ActionGroup.CreateSilently(jActionGroup.Get("Name"), configuration, _navigationService);
            actionGroup.Actions.AddRange(jActionGroup.GetJArray("Actions").Select(j => CreateAction(j, configuration)));
            return actionGroup;
        }

        private ServiceAction CreateAction(JObject jAction, ServiceConfiguration configuration)
        {
            return ServiceAction.CreateSilently(
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
