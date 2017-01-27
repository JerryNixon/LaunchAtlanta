using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.Media.FaceAnalysis;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using System.Linq;
using LaunchApp.Services.CameraService;

namespace LaunchApp.Controls
{
    public sealed partial class KioskPreview : Page
    {
        private CameraService cameraService;

        public KioskPreview()
        {
            this.InitializeComponent();
            Loaded += KioskPreview_Loaded;
            Unloaded += KioskPreview_Unloaded;
        }

        private async void KioskPreview_Loaded(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.Current.Suspending += Application_Suspending;
            var camera = await DeviceService.Instance.GetCameraAsync();
            cameraService = CameraService.Instance;
            await cameraService.InitializeAsync(camera.Id);
            await cameraService.Preview.StartAsync(PreviewControl);
        }

        private async void KioskPreview_Unloaded(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await cameraService?.StopEverythingAsync();
            App.Current.Suspending -= Application_Suspending;
        }

        private async void Application_Suspending(Object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await cameraService?.StopEverythingAsync();
            deferral.Complete();
        }

        public async Task<SoftwareBitmap> CapturePhoto()
        {
            return await cameraService.Photo.CaptureAsync();
        }

        public async Task<StorageFile> CapturePhoto(StorageFolder folder, string name)
        {
            return await cameraService.Photo.CaptureAsync(folder, name);
        }

    }
}
