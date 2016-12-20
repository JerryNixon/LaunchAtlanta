using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Microsoft.FaceSdk.Image;

namespace LaunchFaceSwapLibrary
{
    public static class ConvertTo
    {
        public static class Image
        {
            public static Image<byte> FromSoftwareBitmap(Windows.Graphics.Imaging.SoftwareBitmap bitmap)
            {
                var bytes = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
                bitmap.CopyToBuffer(bytes.AsBuffer());
                return new Image<byte>(bytes, bitmap.PixelWidth, bitmap.PixelHeight, 4, bitmap.PixelWidth * 4, false);
            }
        }

        public static class SoftwareBitmapSource
        {
            public static async Task<Windows.UI.Xaml.Media.Imaging.SoftwareBitmapSource> FromSoftwareBitmap(Windows.Graphics.Imaging.SoftwareBitmap bitmap)
            {
                var source = new Windows.UI.Xaml.Media.Imaging.SoftwareBitmapSource();
                await source.SetBitmapAsync(bitmap);
                return source;
            }
        }

        public static class SoftwareBitmap
        {
            public static Windows.Graphics.Imaging.SoftwareBitmap FromImage(Image<byte> image, int? width = null, int? height = null)
            {
                var bitmap = new Windows.Graphics.Imaging.SoftwareBitmap(BitmapPixelFormat.Unknown, width ?? image.Width, width ?? image.Height);
                bitmap.CopyFromBuffer(image.Pixels.AsBuffer());
                return bitmap;
            }

            public static Windows.Graphics.Imaging.SoftwareBitmap FromSoftwareBitmap(Windows.Graphics.Imaging.SoftwareBitmap bitmap)
            {
                var copy = new Windows.Graphics.Imaging.SoftwareBitmap(bitmap.BitmapPixelFormat, bitmap.PixelWidth, bitmap.PixelHeight);
                bitmap.CopyTo(copy);
                return copy;
            }

            public static async Task<Windows.Graphics.Imaging.SoftwareBitmap> FromFilePathAsync(string path)
            {
                var file = await StorageFile.GetFileFromPathAsync(path);
                var open = await file.OpenAsync(FileAccessMode.Read);
                var decoder = await BitmapDecoder.CreateAsync(open);
                return await decoder.GetSoftwareBitmapAsync();
            }
        }
    }
}
