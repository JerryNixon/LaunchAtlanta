using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace LaunchApp.Services.CameraService
{
    public class PhotoClass
    {
        MediaCapture DefaultManager => CameraService.DefaultManager;

        public PhotoClass()
        {
        }

        public event EventHandler<StorageFile> CapturePhotoComplete;

        public async Task<StorageFile> CaptureAsync(string subfolder = "CameraService")
        {
            if (DefaultManager == null)
                throw new Exception("Not initialized");
            try
            {
                var format = ImageEncodingProperties.CreateJpeg();
                var folder = ApplicationData.Current.LocalCacheFolder;
                var name = $"{DateTime.Now.ToString("yymmddhhnnss")}.jpg";
                var file = await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
                await DefaultManager.CapturePhotoToStorageFileAsync(format, file);
                CapturePhotoComplete?.Invoke(this, file);
                return file;
            }
            catch
            {
                throw;
            }
        }
    }
}
