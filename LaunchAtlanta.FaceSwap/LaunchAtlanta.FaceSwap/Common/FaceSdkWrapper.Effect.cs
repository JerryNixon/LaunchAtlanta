using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.FaceSdk.Effect;
using Microsoft.FaceSdk.Effect.Morph;
using Microsoft.FaceSdk.Effect.Speech;

namespace Microsoft.FaceSdk.Wrapper
{
    /// <summary>
    /// A wrapper for face sdk components, which provides handy methods to create face sdk components.
    /// </summary>
    public static partial class FaceSdkWrapper
    {
        /// <summary>
        /// Create an expression avatar
        /// </summary>
        /// <returns>Expression avatar</returns>
        public static Avatar CreateExpressionAvatar()
        {
            using (var s = ResourceManager.GetStream(ResourceKey.Expression))
            {
                ExpressionAvatar avatar = new ExpressionAvatar(s);
                return avatar;
            }
        }

        /// <summary>
        /// Create a face warp component with mouth blending.
        /// </summary>
        /// <returns>Face warp component</returns>
        public static FaceXform CreateFaceXform()
        {
            using (var s = ResourceManager.GetStream(ResourceKey.Mouth))
            {
                var xform = new WholeFaceXformWithMouth(s);
                return xform;
            }
        }

        /// <summary>
        /// Create a motion generator
        /// </summary>
        /// <returns>Motion generator</returns>
        public static MotionGenerator CreateMotionGenerator()
        { 
            using(var faceS = ResourceManager.GetStream(ResourceKey.FacialMotion))
            using(var headS = ResourceManager.GetStream(ResourceKey.HeadRotation))
            using(var visemeS = ResourceManager.GetStream(ResourceKey.Viseme))
            using(var phoneS = ResourceManager.GetStream(ResourceKey.PhoneMapZhCN))
            {
                var generator = new MotionGenerator(faceS, headS, visemeS, phoneS);
                return generator;
            }
        }

        /// <summary>
        /// Create a converter to convert EnUS phone to string.
        /// </summary>
        /// <returns>Phone converter</returns>
        public static EnUSPhoneConverter CreateEnUSPhoneConverter()
        {
            using (var phone20S = ResourceManager.GetStream(ResourceKey.PhoneMapEnUsVer20))
            using (var phone30S = ResourceManager.GetStream(ResourceKey.PhoneMapEnUsVer30))
            {
                var converter = new EnUSPhoneConverter(phone20S, phone30S);
                return converter;
            }
        }
    }
}
