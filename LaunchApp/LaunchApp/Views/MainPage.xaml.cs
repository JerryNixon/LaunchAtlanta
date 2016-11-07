using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace LaunchApp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private async void BackgroundList_ItemClick(Object sender, ItemClickEventArgs e)
        {
            await MyContentDialog.ShowAsync();
        }

        private void MyContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // TODO
        }

        private void MyContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // TODO
        }
    }
}
