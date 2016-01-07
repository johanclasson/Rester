using GalaSoft.MvvmLight;

namespace Rester.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDeviceStore _deviceStore;

        public MainViewModel(IDeviceStore deviceStore)
        {
            _deviceStore = deviceStore;
        }

        private string _text = "Some dummy text";
        public string Text
        {
            get { return _text; }
            set { Set(nameof(Text), ref _text, value); }
        }
    }
}