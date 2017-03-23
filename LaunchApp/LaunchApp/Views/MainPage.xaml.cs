using LaunchApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Common;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace LaunchApp.Views
{
    public sealed partial class MainPage : Page
    {
        

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            PreviewControl.FaceDetected += PreviewControl_FaceDetected;
            Loaded += MainPage_Loaded;
        }

        DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        private void MainPage_Loaded(Object sender, RoutedEventArgs e)
        {
            timer.Tick += (s, args) =>
            {
                if (VisualStateGroup.CurrentState == VisualStateAdmin)
                {
                    return;
                }
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                if (VisualStateGroup.CurrentState != VisualStateNormal)
                {
                    VisualStateManager.GoToState(this, VisualStateIdle.Name, true);
                }
            };
        }

        private void PreviewControl_FaceDetected(Object sender, CameraControl.FaceDetectedEventArgsEx e)
        {
            if (VisualStateGroup.CurrentState == VisualStateAdmin)
            {
                return;
            }
            if (e.Faces.Any() && VisualStateGroup.CurrentState != VisualStateNormal)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                VisualStateManager.GoToState(this, VisualStateNormal.Name, true);
            }
            else
            {
                if (!timer.IsEnabled)
                {
                    timer.Start();
                }
            }
        }

        private async void BackgroundList_ItemClick(Object sender, ItemClickEventArgs e)
        {
            try
            {
                var foreground = await PreviewControl.CapturePhotoAsync(ApplicationData.Current.TemporaryFolder, Guid.NewGuid().ToString());
                var blobService = new Services.BlobService.BlobService(accountName, blobKey, "launch", "face");
                await blobService.UploadAsync(foreground);
                var payload = new FaceSwapDetails() { key = swapKey, envfilename = e.ClickedItem as string, facefilename = foreground.Name };
                new Services.FaceSwapService().PostAsync(payload);

                var contentDialog = new ContentDialog
                {
                    Title = "Message",
                    Content = "Done",
                    PrimaryButtonText = "Ok"
                };
                await contentDialog.ShowAsync();
            }
            finally
            {
                // TODO
            }

        }

        private void SettingsButton_Click(Object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, VisualStateAdmin.Name, true);
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;
            var b = new BitmapImage(new Uri($"https://launchfaceswapstor.blob.core.windows.net/env/{image.Tag}?st=2017-03-23T21%3A18%3A00Z&se=2018-03-22T21%3A18%3A00Z&sp=rl&sv=2015-12-11&sr=c&sig=81BMu69pNOU4oiBRyEjSQl6iSywLCTEXpnFMVS3OET4%3D"));
            image.Source = b;
        }
    }

}
