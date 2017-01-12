using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.FaceAnalysis;
using Microsoft.FaceSdk.Common;
using Microsoft.FaceSdk.Image;
using Microsoft.FaceSdk.Alignment;
using Microsoft.FaceSdk.Wrapper;
using Microsoft.FaceSdk.Effect.Swap;
using System.Diagnostics;

namespace LaunchFaceSwapLibrary
{
    public class LaunchFaceSwapService
    {
        static IFaceAlignmentor alignmentor;
        static FaceSwapper swapper = new FaceSwapper();

        public async Task<SoftwareBitmap> SwapFacesAsync(SoftwareBitmap background, SoftwareBitmap foreground)
        {
            var backgroundPrep = await PrepBitmapAsync(background);
            if (backgroundPrep == null)
            {
                return null;
            }
            var backgroundFaces = backgroundPrep.Item1;
            var backgroundPoints = backgroundPrep.Item2;

            var foregroundPrep = await PrepBitmapAsync(foreground);
            if (foregroundPrep == null)
            {
                return null;
            }
            var foregroundFaces = foregroundPrep.Item1;
            var foregroundPoints = foregroundPrep.Item2;

            try
            {
                var resultingImage = default(Image<byte>);
                swapper.Swap3D(backgroundFaces, backgroundPoints, foregroundFaces, foregroundPoints, resultingImage);
                return ConvertTo.SoftwareBitmap.FromImage(resultingImage);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }
        }

        private async Task<Tuple<Image<byte>, IList<PointF>>> PrepBitmapAsync(SoftwareBitmap bitmap)
        {
            if (bitmap.PixelHeight % 2 != 0)
            {
                var resized = new SoftwareBitmap(bitmap.BitmapPixelFormat, bitmap.PixelWidth, bitmap.PixelHeight + 1);
                bitmap.CopyTo(resized);
                bitmap = resized;
            }

            Rectangle firstFace;
            try
            {
                var detector = await FaceDetector.CreateAsync();
                var formats = FaceDetector.GetSupportedBitmapPixelFormats();
                var convertedBitmap = SoftwareBitmap.Convert(bitmap, formats.First());
                var detected = await detector.DetectFacesAsync(convertedBitmap);
                var faces = detected
                    .Select(x => x.FaceBox)
                    .Select(x => new Rectangle((int)x.X, (int)x.X + (int)x.Width, (int)x.Y, (int)x.Y + (int)x.Height));
                if (!faces.Any())
                {
                    return null;
                }
                firstFace = faces.First();
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }

            IList<PointF> points;
            var image = ConvertTo.Image.FromSoftwareBitmap(bitmap);
            try
            {
                if (alignmentor == null)
                {
                    using (var stream = ResourceManager.GetStream(ResourceKey.AsmAlignment))
                    {
                        alignmentor = FaceAlignmentorFactory.Create(FaceAlignmentType.Asm87Points, stream);
                    }
                }
                var grayImage = new ImageGray(image);
                points = alignmentor.Align(grayImage, firstFace).ToList();
                if (!points.Any())
                {
                    return null;
                }
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }

            return new Tuple<Image<byte>, IList<PointF>>(image, points);
        }
    }
}