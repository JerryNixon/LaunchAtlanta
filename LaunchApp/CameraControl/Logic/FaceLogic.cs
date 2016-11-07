using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace CameraControl
{
    public class FaceLogic
    {
        public Boolean IsEnabled { get; set; }

        private FaceDetectionEffect currentEffect;
        private MediaCapture defaultManager;

        public FaceLogic(MediaCapture defaultManager)
        {
            this.defaultManager = defaultManager;
        }

        public event EventHandler<FaceDetectedEventArgs> FaceDetected;
        public FrameworkElement FacesCanvas { get; } = new Viewbox
        {
            Child = new Canvas(),
            Stretch = Stretch.Uniform,
        };

        public async Task InitAsync()
        {
            if (currentEffect != null)
            {
                currentEffect.Enabled = false;
                currentEffect.FaceDetected -= FaceDetection_FaceDetected;
            }
            var definition = new FaceDetectionEffectDefinition
            {
                SynchronousDetectionEnabled = false,
                DetectionMode = FaceDetectionMode.HighPerformance,
            };
            currentEffect = await defaultManager.AddVideoEffectAsync(definition, MediaStreamType.VideoPreview) as FaceDetectionEffect;
            currentEffect.DesiredDetectionInterval = TimeSpan.FromMilliseconds(150);
            currentEffect.FaceDetected += FaceDetection_FaceDetected;
        }

        private void FaceDetection_FaceDetected(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            FaceDetected?.Invoke(sender, args);
        }
    }
}