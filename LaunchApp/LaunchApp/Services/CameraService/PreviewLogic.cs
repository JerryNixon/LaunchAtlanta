using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml.Controls;

namespace LaunchApp.Services.CameraService
{
    public class PreviewLogic
    {
        private MediaCapture defaultManager;
        private FaceDetectionEffect FaceDetection => CameraService.FaceDetection;

        public bool Previewing => defaultManager.CameraStreamState == Windows.Media.Devices.CameraStreamState.Streaming;
        public VideoEncodingProperties VideoEncodingProperties => defaultManager.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

        public PreviewLogic(MediaCapture defaultManager)
        {
            this.defaultManager = defaultManager;
        }

        public async Task StartAsync(CaptureElement element)
        {
            if (Previewing)
            {
                throw new Exception("Already previewing");
            }
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            FaceDetection.Enabled = true;
            element.Source = defaultManager;
            await defaultManager.StartPreviewAsync();
        }

        public async Task StopAsync()
        {
            if (!Previewing)
            {
                throw new Exception("Not previewing");
            }
            FaceDetection.Enabled = false;
            await defaultManager.StopPreviewAsync();
        }
    }
}
