using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.System.Display;

namespace CameraControl
{
    public class CameraService
    {
        public FaceLogic Face { get; private set; }
        public PreviewLogic Preview { get; private set; }
        public PhotoLogic Photo { get; private set; }
        public VideoLogic Video { get; private set; }

        public event EventHandler<FaceDetectedEventArgs> FaceDetected;

        private DisplayRequest DisplayRequest { get; } = new DisplayRequest();
        public static MediaCapture DefaultManager { get; set; } = null;
        public string VideoId => DefaultManager?.MediaCaptureSettings.VideoDeviceId;
        public string AudioId => DefaultManager?.MediaCaptureSettings.VideoDeviceId;

        public async Task<MediaCapture> InitAsync(string videoDeviceId, string audioDeviceId = null)
        {
            // Prevent the screen from sleeping 
            DisplayRequest.RequestActive();

            if (DefaultManager != null)
            {
                var current = DefaultManager.MediaCaptureSettings.VideoDeviceId?.ToLower();
                if (current.Equals(videoDeviceId))
                {
                    return DefaultManager;
                }
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

            Face = new FaceLogic(DefaultManager);
            Face.FaceDetected += Face_FaceDetected;
            await Face.InitAsync();

            Preview = new PreviewLogic(DefaultManager, Face);
            Photo = new PhotoLogic(DefaultManager);
            Video = new VideoLogic(DefaultManager);

            return DefaultManager;
        }

        public async Task StopEverythingAsync()
        {
            if (Preview?.IsPreviewing ?? false)
            {
                await Preview.StopAsync();
            }

            if (Video?.IsCapturing ?? false)
            {
                await Video.StopCaptureAsync();
            }

            if (Face != null)
            {
                Face.IsEnabled = false;
                Face.FaceDetected -= Face_FaceDetected;
            }

            if (DefaultManager != null)
            {
                DefaultManager.RecordLimitationExceeded -= DefaultManager_RecordLimitationExceeded;
                await DefaultManager.ClearEffectsAsync(MediaStreamType.VideoPreview);
                DefaultManager.Dispose();
                DefaultManager = null;
            }
        }

        private void Face_FaceDetected(Object sender, FaceDetectedEventArgs e)
        {
            FaceDetected?.Invoke(sender, e);
        }

        private async void DefaultManager_RecordLimitationExceeded(MediaCapture sender)
        {
            await Video.StopCaptureAsync();
        }
    }
}
