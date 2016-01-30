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
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IHttpClient, HttpClientFactory>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IServiceStore, DesignServiceStore>();
                SimpleIoc.Default.Register<ILogStore, DesignLogStore>();
            }
            else
            {
                SimpleIoc.Default.Register<IServiceStore, DesignServiceStore>();
                SimpleIoc.Default.Register<ILogStore, DesignLogStore>();
                //SimpleIoc.Default.Register<IServiceStore, ServiceStore>();
                //SimpleIoc.Default.Register<ILogStore, LogStore>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LogViewModel>();
            SimpleIoc.Default.GetInstance<LogViewModel>(); //Create immediately
        }

        private static INavigationService CreateNavigationService()
        {
            var nc = new NavigationService();
            nc.Configure(LogPage.Key, typeof(LogPage));
            return nc;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public LogViewModel Log => ServiceLocator.Current.GetInstance<LogViewModel>();
    }
}
