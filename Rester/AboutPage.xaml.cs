using System;
using Windows.UI.Xaml.Input;

namespace Rester
{
    public sealed partial class AboutPage
    {
        public AboutPage()
        {
            InitializeComponent();
            VersionTextBlock.Text = ApplicationVersion;
        }

        public static string Key => nameof(AboutPage);

        public string ApplicationVersion
        {
            get
            {
                var ver = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
            }
        }

        private async void OnGoToProjectHomePageTapped(object sender, TappedRoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/johanclasson/Rester"));
        }
    }
}
