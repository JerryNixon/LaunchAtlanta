using System;
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

        private async void MyContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // TODO
        }

        private void MyContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // TODO
        }
    }
}
