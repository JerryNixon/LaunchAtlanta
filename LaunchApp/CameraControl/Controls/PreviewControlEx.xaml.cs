using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.FaceAnalysis;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace CameraControl
{
    public sealed partial class PreviewControlEx : Page
    {
        private CameraService cameraService;
        private CoreDispatcher dispatcher;

        public PreviewControlEx()
        {
            InitializeComponent();
            Loaded += PreviewControlEx_Loaded;
            Unloaded += PreviewControlEx_Unloaded;
            FaceOutlineStyle = DefaultFaceOutlineStyle;
        }

        private async void PreviewControlEx_Loaded(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            dispatcher = Dispatcher;
            var camera = await DeviceService.Instance.GetCameraAsync();
            cameraService = new CameraService();
            await cameraService.InitAsync(camera.Id);
            await cameraService.Preview.StartAsync(MyPreviewControl);
            cameraService.FaceDetected += CameraService_FaceDetected;
        }

        private async void PreviewControlEx_Unloaded(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await cameraService?.StopEverythingAsync();
            cameraService.FaceDetected -= CameraService_FaceDetected;
        }

        private async void CameraService_FaceDetected(Object sender, Windows.Media.Core.FaceDetectedEventArgs e)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => DrawFaceOutlines(e));
        }

        public event EventHandler<FaceDetectedEventArgsEx> FaceDetected;

        private async void DrawFaceOutlines(Windows.Media.Core.FaceDetectedEventArgs e)
        {
            FaceOutlineCanvas.Children.Clear();
            var faces = e.ResultFrame.DetectedFaces?.Where(x => x.FaceBox.Width != 0 && x.FaceBox.Height != 0);
            foreach (var face in faces)
            {
                var outline = new Rectangle
                {
                    Style = FaceOutlineStyle,
                    Height = face.FaceBox.Height,
                    Width = face.FaceBox.Width,
                };
                Canvas.SetLeft(outline, face.FaceBox.X);
                Canvas.SetTop(outline, face.FaceBox.Y);
                FaceOutlineCanvas.Children.Add(outline);

                //var pic = await cameraService.Photo.CaptureAsync();
                //uint startPointX = (uint)Math.Floor(face.FaceBox.X * scale);
                //uint startPointY = (uint)Math.Floor(face.FaceBox.Y * scale);
                //uint height = (uint)Math.Floor(corpSize.Height * scale);
                //uint width = (uint)Math.Floor(corpSize.Width * scale);
            }

            var projection = faces.Select(x => new DetectedFaceEx { FaceBox = x.FaceBox });
            var args = new FaceDetectedEventArgsEx { Faces = projection };
            FaceDetected?.Invoke(this, args);
        }

        public Size PhotoResolution => cameraService.PhotoResolution;

        public Size VideoResolution => cameraService.VideoResolution;

        public Style FaceOutlineStyle { get; set; }

        public async Task<SoftwareBitmap> CapturePhotoAsync() => await cameraService.Photo.CaptureAsync();

        public async Task<StorageFile> CapturePhotoAsync(StorageFolder folder, string name) => await cameraService.Photo.CaptureAsync(folder, name);

    }

    public class DetectedFaceEx
    {
        public object Image { get; set; }
        public BitmapBounds FaceBox { get; set; }
    }

    public class FaceDetectedEventArgsEx : EventArgs
    {
        public IEnumerable<DetectedFaceEx> Faces { get; set; }
    }
}
