using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Utils;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.Devices;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.Search;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace LaunchApp.Services.CameraService
{
    public class FaceLogic
    {
        public static event EventHandler<int> FaceCountChanged;
        public static FaceDetectionEffect FaceDetection;

        private int _previousFaceCount = 0;
        private MediaCapture defaultManager;

        public FaceLogic(MediaCapture defaultManager)
        {
            this.defaultManager = defaultManager;
        }

        public event EventHandler<FaceDetectedEventArgs> FaceDetected;
        public Color FacesBoxColor { get; set; } = Colors.Yellow;
        public Viewbox FacesCanvas { get; } = new Viewbox
        {
            Child = new Canvas(),
            Stretch = Windows.UI.Xaml.Media.Stretch.Uniform,
        };

        private async Task<FaceDetectionEffect> SetupFaceDetection()
        {
            var faceDetectionEffectDefinition = new FaceDetectionEffectDefinition
            {
                SynchronousDetectionEnabled = false,
                DetectionMode = FaceDetectionMode.HighPerformance,
            };
            var faceDetection = await defaultManager.AddVideoEffectAsync(faceDetectionEffectDefinition, MediaStreamType.VideoPreview) as FaceDetectionEffect;
            faceDetection.FaceDetected += FaceDetection_FaceDetected;
            faceDetection.DesiredDetectionInterval = TimeSpan.FromMilliseconds(33);
            if (FaceDetection != null)
            {
                FaceDetection.Enabled = false;
            }
            return FaceDetection = faceDetection;
        }

        private void FaceDetection_FaceDetected(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            DispatcherWrapper.Current().Dispatch(() =>
            {
                var properties = defaultManager.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
                if (properties == null || properties.Width == 0 || properties.Height == 0)
                    return;

                var canvas = FacesCanvas.Child as Canvas;
                canvas.Height = properties.Height;
                canvas.Width = properties.Width;
                FaceDetected?.Invoke(sender, args);
                canvas.Children.Clear();

                foreach (var face in args.ResultFrame.DetectedFaces.Where(x => x.FaceBox.Width != 0 && x.FaceBox.Height != 0))
                {
                    var box = new Rectangle
                    {
                        Height = face.FaceBox.Height,
                        Width = face.FaceBox.Width,
                        Stroke = FacesBoxColor.ToSolidColorBrush(),
                        StrokeThickness = 2,
                    };
                    Canvas.SetLeft(box, face.FaceBox.X);
                    Canvas.SetTop(box, face.FaceBox.Y);
                    canvas.Children.Add(box);
                }
                FaceCountChanged?.Invoke(this, canvas.Children.Count());
            });
        }
    }
}