using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Rester.Service;

namespace Rester.ViewModel
{
    internal class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var navigationService = CreateNavigationService();

            SimpleIoc.Default.Register(() => navigationService);
            SimpleIoc.Default.Register<IDialog, Dialog>();
            SimpleIoc.Default.Register<IActionInvokerFactory, ActionInvokerFactory>();
            SimpleIoc.Default.Register<IDeserializer, Deserializer>();
            SimpleIoc.Default.Register<ISerializer, Serializer>();
            SimpleIoc.Default.Register<IZipper, Zipper>();
            SimpleIoc.Default.Register<IFilePicker, FilePicker>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IConfigurationStore, DesignConfigurationStore>();
                SimpleIoc.Default.Register<ILogStore, DesignLogStore>();
            }
            else
            {
                //SimpleIoc.Default.Register<IConfigurationStore, DesignConfigurationStore>();
                SimpleIoc.Default.Register<IConfigurationStore, ConfigurationStore>();
                SimpleIoc.Default.Register<ILogStore, LogStore>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LogViewModel>();
            SimpleIoc.Default.GetInstance<LogViewModel>(); //Create immediately
        }

        private static INavigationService CreateNavigationService()
        {
            var nc = new NavigationService();
            nc.Configure(LogPage.Key, typeof(LogPage));
            nc.Configure(ActionPage.Key, typeof(ActionPage));
            nc.Configure(AboutPage.Key, typeof(AboutPage));
            return nc;
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public LogViewModel Log => ServiceLocator.Current.GetInstance<LogViewModel>();
    }
}
