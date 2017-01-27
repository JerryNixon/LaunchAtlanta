using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace LaunchApp.Services.CameraService
{
    public class PhotoLogic
    {
        private MediaCapture defaultManager;

        public PhotoLogic(MediaCapture defaultManager)
        {
            this.defaultManager = defaultManager;
        }

        public async Task<SoftwareBitmap> CaptureAsync()
        {
            try
            {
                var lowLagCapture = await defaultManager.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));
                var capturedPhoto = await lowLagCapture.CaptureAsync();
                var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;
                await lowLagCapture.FinishAsync();
                return softwareBitmap;
            }
            catch
            {
                throw;
            }
        }

        public async Task<StorageFile> CaptureAsync(StorageFolder folder, string name = "capture.jpg")
        {
            try
            {
                folder = folder ?? ApplicationData.Current.TemporaryFolder;
                var format = ImageEncodingProperties.CreateJpeg();
                var collide = CreationCollisionOption.GenerateUniqueName;
                var file = await folder.CreateFileAsync(name, collide);
                await defaultManager.CapturePhotoToStorageFileAsync(format, file);
                return file;
            }
            catch
            {
                throw;
            }
        }
    }
}
