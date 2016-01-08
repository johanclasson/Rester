using System;
using GalaSoft.MvvmLight;
using Rester.Model;

namespace Rester.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IServiceStore _serviceStore;

        public MainViewModel(IServiceStore serviceStore)
        {
            _serviceStore = serviceStore;
            LoadData();
        }

        private async void LoadData()
        {
            var configs = await _serviceStore.GetServiceConfigurations();
            ServiceConfigurations.ClearAndAddRange(configs);
        }

        public ObservableCollection2<ServiceConfiguration> ServiceConfigurations { get; } = new ObservableCollection2<ServiceConfiguration>();
    }
}