using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.FaceSdk.Alignment;
using Microsoft.FaceSdk.Common;
using Microsoft.FaceSdk.Detection;
using Microsoft.FaceSdk.Effect.Swap;
using Microsoft.FaceSdk.Image;
using Microsoft.FaceSdk.Image.Platform;
using Microsoft.FaceSdk.Reconstruction;
using Microsoft.FaceSdk.Wrapper;
using DWG = System.Drawing;
using IMG = System.Drawing.Imaging;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LaunchAtlanta.FaceSwap
{
    public class Swapper
    {
        static IFaceDetector detector = FaceSdkWrapper.CreateDetector(FaceDetectionType.Multiview);
        static IFaceAlignmentor alignmentor = FaceSdkWrapper.CreateAlignmentor(FaceAlignmentType.Asm87Points);
        static FaceReconstructor reconstructor = FaceSdkWrapper.CreateReconstructor();
        static FaceSwapper swapper = new FaceSwapper();

        public string SwapFaces(string sourceFaceFilePath, string targetFaceFilePath, string outputFaceFilePath)
        {
            Image<byte> source = ImageReader.LoadImage(sourceFaceFilePath);
            Image<byte> target = ImageReader.LoadImage(targetFaceFilePath);
            Image<byte> sourceSwapImage = source;
            Image<byte> output;

            FaceSwapper.MatchTargetSkinColor = true;
            FaceSwapper.Enable3dSwap = true;
            FaceSwapper.FaceRectangleScaleFactor = new SizeF(1, 1.4f);
            SwapHelper.SuggestedSwapSize = 200;
            FaceSwapper.ApplyFaceAsMask = true;

            swapper.Reconstructor = reconstructor;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer outputContainer = blobClient.GetContainerReference("output");
            CloudBlockBlob outputBlob = outputContainer.GetBlockBlobReference(outputFaceFilePath.Split('\\')[2]);

            try
            {
                FaceSwapInTwoImages(sourceSwapImage, target, source, out output);
                output.Save(outputFaceFilePath);

                using (var fileStream = File.OpenRead(outputFaceFilePath))
                {
                    outputBlob.UploadFromStream(fileStream);
                }

                return "Success: Face swap was output saved to disk";
            }
            catch (Exception up)
            {
                return "Error: Unable to perform face swap.";
            }
        }

        private static int imgCount;

        private void FaceSwapInTwoImages(
                Image<byte> argbImage1,
                Image<byte> argbImage2,
                Image<byte> facepointSource,
                out Image<byte> swapResult1)
        {
            if (argbImage1 == null || argbImage1.Channels < Color.RgbChannels)
                throw new ArgumentException("Input image 1 should be a valid color image.");

            if (argbImage2 == null || argbImage2.Channels < Color.RgbChannels)
                throw new ArgumentException("Input image 2 should be a valid color image.");

            // face detection
            Image<byte> imgGray1 = new ImageGray(facepointSource);  //argbImage1

            Stopwatch sw = Stopwatch.StartNew();
            Stopwatch sw2 = Stopwatch.StartNew();
            var faces1 = detector.Detect(imgGray1);

            if (faces1.Length < 1)
            {
                throw new InvalidOperationException("Fail to detect face in image 1.");
            }


            // pick two largest faces
            var face1 = faces1.OrderByDescending(y => y.Rect.Area).First();

            Image<byte> imgGray2 = new ImageGray(argbImage2);

            sw.Restart();
            var faces2 = detector.Detect(imgGray2);

            if (faces2.Length < 1)
            {
                throw new InvalidOperationException("Fail to detect face in image 2.");
            }

            var face2 = faces2.OrderByDescending(y => y.Rect.Area).First();

            sw.Restart();
            var alignmentShape1 = alignmentor.Align(imgGray1, face1.Rect);

            sw.Restart();
            var alignmentShape2 = alignmentor.Align(imgGray2, face2.Rect);

            // create the face point file
            FaceSwapper.SaveTestData("points-Source", argbImage1, alignmentShape1);
            FaceSwapper.SaveTestData("facepoints-Source", facepointSource, alignmentShape1);
            FaceSwapper.SaveTestData("points-target", argbImage2, alignmentShape2);

            swapResult1 = argbImage2.Clone();

            sw.Restart();
            FaceSwapInternal(argbImage1, argbImage2, alignmentShape1, alignmentShape2, swapResult1);
        }

        private DWG.Brush GetPointColor(int index)
        {
            return DWG.Brushes.LightGray;
        }

        private void CreatePointImage(PointF[] alignmentShape1, string srcFilename, string outfilename, bool shape = false)
        {
            var stream = File.OpenRead(srcFilename);
            var bmp = new DWG.Bitmap(stream);
            stream.Close();
            var font = new DWG.Font(DWG.FontFamily.GenericSansSerif, 12);
            using (DWG.Graphics g = DWG.Graphics.FromImage(bmp))
            {
                g.InterpolationMode = DWG.Drawing2D.InterpolationMode.HighQualityBicubic;
                if (shape)
                    g.DrawPolygon(new DWG.Pen(DWG.Brushes.LightGray, 4), alignmentShape1.Select(p => new DWG.PointF(p.X, p.Y)).ToArray());
                else
                {
                    for (int i = 0; i < alignmentShape1.Length; i++)
                    {
                        var pnt = alignmentShape1[i];
                        g.FillEllipse(GetPointColor(i), pnt.X - 1.5f, pnt.Y - 1.5f, 3, 3);
                        g.DrawString(i.ToString(), font, DWG.Brushes.LightGray, new DWG.PointF(pnt.X - 12f, pnt.Y - 6f));
                    }
                }
            }

            bmp.Save(outfilename, IMG.ImageFormat.Jpeg);
            bmp.Dispose();
        }


        /// <summary>
        /// Swap two faces and dump result.
        /// </summary>
        /// <param name="face1"></param>
        /// <param name="face2"></param>
        /// <param name="faceShape1"></param>
        /// <param name="faceShape2"></param>
        /// <param name="swapResult1">Swap result image, should be same as face1</param>
        private void FaceSwapInternal(
            Image<byte> face1,
            Image<byte> face2,
            IList<PointF> faceShape1,
            IList<PointF> faceShape2,
            Image<byte> swapResult1)
        {
            Stopwatch sw = Stopwatch.StartNew();

            // swap face 1 to face 2
            swapper.Swap3D(face1, faceShape1, face2, faceShape2, swapResult1);

            sw.Stop();
            long time1 = sw.ElapsedMilliseconds;

            sw.Restart();

            // swap face 2 to face 1
            //swapper.Swap3D(face2, faceShape2, face1, faceShape1, swapResult1);

            sw.Stop();
            long time2 = sw.ElapsedMilliseconds;
        }
    }
}
