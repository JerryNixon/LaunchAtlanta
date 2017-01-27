using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace CameraControl
{
    public class VideoLogic
    {
        private StorageFile captureVideoFile;
        private MediaCapture defaultManager;

        public bool IsCapturing => captureVideoFile != null;

        public VideoLogic(MediaCapture defaultManager)
        {
            this.defaultManager = defaultManager;
        }

        public async Task StartCaptureAsync(StorageFolder folder, string name = "video.mp4")
        {
            if (IsCapturing)
            {
                throw new InvalidOperationException("Already capturing");
            }
            var profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);
            captureVideoFile = await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
            await defaultManager.StartRecordToStorageFileAsync(profile, captureVideoFile);
        }

        public async Task<StorageFile> StopCaptureAsync()
        {
            if (!IsCapturing)
            {
                throw new InvalidOperationException("Not capturing");
            }
            try
            {
                await defaultManager.StopRecordAsync();
                return captureVideoFile;
            }
            finally
            {
                captureVideoFile = null;
            }
        }
    }
}
