using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LaunchAtlanta.FaceSwap.Tests
{
    [TestClass]
    public class SwapperTests
    {
        [TestMethod]
        public void TestFaceSwap()
        {
            var source = @"c:\tmp\donald.jpg";
            var target = @"c:\tmp\hillary.jpg";
            var output = @"c:\tmp\lib_output.jpg";

            var swapper = new Swapper();
            var result = swapper.SwapFaces(source, target, output);

            Assert.AreEqual("Success: Face swap was output saved to disk", result);
        }
    }
}
