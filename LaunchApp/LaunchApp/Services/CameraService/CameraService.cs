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
    public class CameraService
    {
        public static CameraService Instance;

        static CameraService()
        {
            Instance = new CameraService();
        }

        private CameraService()
        {
            // Prevent the screen from sleeping 
            DisplayRequest.RequestActive();
        }

        private DisplayRequest DisplayRequest { get; } = new DisplayRequest();
        public static MediaCapture DefaultManager { get; set; } = null;
        public string VideoId => DefaultManager?.MediaCaptureSettings.VideoDeviceId;
        public string AudioId => DefaultManager?.MediaCaptureSettings.VideoDeviceId;

        public async Task<MediaCapture> InitializeAsync(string videoDeviceId, string audioDeviceId = null)
        {
            if (DefaultManager != null && DefaultManager.MediaCaptureSettings.VideoDeviceId.ToLower() == videoDeviceId.ToLower())
            {
                return DefaultManager;
            }

            try
            {
                await StopEverythingAsync();
            }
            catch { }

            DefaultManager = new MediaCapture();
            DefaultManager.RecordLimitationExceeded += DefaultManager_RecordLimitationExceeded;
            var settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
            };
            if (videoDeviceId != null) settings.VideoDeviceId = videoDeviceId;
            if (audioDeviceId != null) settings.AudioDeviceId = audioDeviceId;

            await DefaultManager.InitializeAsync(settings);
            await SetupFaceDetection();

            Face = new FaceLogic(DefaultManager);
            Preview = new PreviewLogic(DefaultManager);
            Photo = new PhotoLogic(DefaultManager);
            Video = new VideoLogic(DefaultManager);


            return DefaultManager;
        }

        private async void DefaultManager_RecordLimitationExceeded(MediaCapture sender)
        {
            await Video.StopCaptureAsync();
        }

        public async Task StopEverythingAsync()
        {
            if (Preview.Previewing)
                await Preview.StopAsync();

            if (Video.Capturing)
                await Video.StopCaptureAsync();

            if (FaceDetection != null)
            {
                FaceDetection.Enabled = false;
                FaceDetection.FaceDetected -= FaceDetection_FaceDetected;
            }

            if (DefaultManager != null)
            {
                DefaultManager.RecordLimitationExceeded -= DefaultManager_RecordLimitationExceeded;
                await DefaultManager.ClearEffectsAsync(MediaStreamType.VideoPreview);
                DefaultManager.Dispose();
                DefaultManager = null;
            }
        }

        #region Face detect

        public FaceLogic Face { get; private set; }

        #endregion

        public PreviewLogic Preview { get; private set; }
        public PhotoLogic Photo { get; private set; }
        public VideoLogic Video { get; private set; }
    }
}
