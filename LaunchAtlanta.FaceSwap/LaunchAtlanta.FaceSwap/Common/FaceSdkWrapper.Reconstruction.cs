using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.FaceSdk.Reconstruction;

namespace Microsoft.FaceSdk.Wrapper
{
    /// <summary>
    /// A wrapper for face sdk components, which provides handy methods to create face sdk components.
    /// </summary>
    public static partial class FaceSdkWrapper
    {

        /// <summary>
        /// Create a face reconstructor.
        /// </summary>
        /// <returns>Face reconstructor</returns>
        public static FaceReconstructor CreateReconstructor()
        {
            using (var ss = ResourceManager.GetStream(ResourceKey.ReconstructionShape))
            using (var ps = ResourceManager.GetStream(ResourceKey.ReconstructionPose))
            {
                var reconstructor = new FaceReconstructor(ss, ps);
                return reconstructor;
            }
        }

    }
}
