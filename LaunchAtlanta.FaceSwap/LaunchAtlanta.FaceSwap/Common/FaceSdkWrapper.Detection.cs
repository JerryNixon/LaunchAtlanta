using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.FaceSdk.Alignment;
using Microsoft.FaceSdk.Detection;

namespace Microsoft.FaceSdk.Wrapper
{
    /// <summary>
    /// A wrapper for face sdk components, which provides handy methods to create face sdk components.
    /// </summary>
    public static partial class FaceSdkWrapper
    {
        /// <summary>
        /// Create face alignmentor
        /// </summary>
        /// <param name="type">Face alignment algorithm type</param>
        /// <returns>Face alignmentor</returns>
        public static IFaceAlignmentor CreateAlignmentor(FaceAlignmentType type)
        {
            ResourceKey key = ResourceKey.Unknown;
            switch (type)
            {
                case FaceAlignmentType.Asm87Points:
                    key = ResourceKey.AsmAlignment;
                    break;
                case FaceAlignmentType.Regression5Points:
                    key = ResourceKey.RegressionAlignment;
                    break;
                case FaceAlignmentType.Regression27Points:
                    key = ResourceKey.RegressionAlignment27Points;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", "Unsupported alignment algorithm type.");
            }

            using (var s = ResourceManager.GetStream(key))
            {
                var alignmentor = FaceAlignmentorFactory.Create(type, s);
                return alignmentor;
            }
        }

        /// <summary>
        /// Create face detector
        /// </summary>
        /// <param name="type">Face detection algorithm type</param>
        /// <returns>Face detector</returns>
        public static IFaceDetector CreateDetector(FaceDetectionType type)
        {
            ResourceKey cascadeKey = ResourceKey.Unknown;
            ResourceKey filterKey = ResourceKey.Unknown;
            switch (type)
            {
                case FaceDetectionType.Frontal:
                    cascadeKey = ResourceKey.FrontalCascade;
                    filterKey = ResourceKey.FrontalPostFilter;
                    break;
                case FaceDetectionType.Multiview:
                    cascadeKey = ResourceKey.MultiviewCascade;
                    filterKey = ResourceKey.MultiviewPostFilter;
                    break;
                case FaceDetectionType.MultiviewPyramid:
                    cascadeKey = ResourceKey.MultiviewCascade;
                    filterKey = ResourceKey.MultiviewPostFilter;
                    break;
                case FaceDetectionType.MultiviewPrefilter:
                    cascadeKey = ResourceKey.MultiviewCascade;
                    filterKey = ResourceKey.MultiviewPostFilter;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", "Unsupported detection algorithm type.");
            }

            using (var cs = ResourceManager.GetStream(cascadeKey))
            using(var ps = ResourceManager.GetStream(filterKey))
            {
                var detector = FaceDetectorFactory.Create(type, cs, ps);
                return detector;
            }
        }

        /// <summary>
        /// Create a skin detector
        /// </summary>
        /// <returns>Skin detector</returns>
        public static SkinDetector CreateSkinDetector()
        {
            using (var s = ResourceManager.GetStream(ResourceKey.Skin))
            {
                var detector = new SkinDetector(s);
                return detector;
            }
        }

        /// <summary>
        /// Create pose regressor.
        /// </summary>
        /// <returns>Pose regressor</returns>
        public static PoseRegressor CreatePoseRegressor()
        {
            using(var stream = ResourceManager.GetStream(ResourceKey.PoseLDAKNN))
            {
                PoseRegressor regressor = new PoseRegressor(stream);
                return regressor;
            }
        }

        /// <summary>
        /// Create gender classifier
        /// </summary>
        /// <returns>Gender classifier</returns>
        public static GenderClassifier CreateGenderClassifier()
        {
            using (var genderS = ResourceManager.GetStream(ResourceKey.Gender))
            {
                return new GenderClassifier(genderS);
            }
        }
    }
}
