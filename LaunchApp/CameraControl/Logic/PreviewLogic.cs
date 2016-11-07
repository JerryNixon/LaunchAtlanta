using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml.Controls;

namespace CameraControl
{
    public class PreviewLogic
    {
        private FaceLogic faceLogic;
        private MediaCapture defaultManager;

        public bool IsPreviewing 
            => defaultManager.CameraStreamState == Windows.Media.Devices.CameraStreamState.Streaming;

        public VideoEncodingProperties VideoEncodingProperties 
            => defaultManager.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

        public PreviewLogic(MediaCapture defaultManager, FaceLogic face)
        {
            this.defaultManager = defaultManager;
            this.faceLogic = face;
        }

        public async Task StartAsync(CaptureElement element)
        {
            if (IsPreviewing)
            {
                throw new Exception("Already previewing");
            }
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            faceLogic.IsEnabled = true;
            element.Source = defaultManager;
            await defaultManager.StartPreviewAsync();
        }

        public async Task StopAsync()
        {
            if (!IsPreviewing)
            {
                throw new Exception("Not previewing");
            }
            faceLogic.IsEnabled = false;
            await defaultManager.StopPreviewAsync();
        }
    }
}