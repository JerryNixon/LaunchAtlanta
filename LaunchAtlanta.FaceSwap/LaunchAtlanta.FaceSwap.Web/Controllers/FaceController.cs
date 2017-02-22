using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using LaunchAtlanta.FaceSwap;
using LaunchAtlanta.FaceSwap.Web.Models;
using Newtonsoft.Json;

namespace LaunchAtlanta.FaceSwap.Web.Controllers
{
    public class FaceController : ApiController
    {
        // GET: api/Face
        public async Task<string> Post()
        {
            var data = await Request.Content.ReadAsStringAsync();
            var requestData = JsonConvert.DeserializeObject<RequestBodyData>(data);

            if (requestData.Key != Constants.Key)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Incorrect Auth Token." };
                throw new HttpResponseException(msg);
            }

            CloudStorage cloudStorage = new CloudStorage();

            string faceFileName = requestData.FaceFileName;
            string envFileName = requestData.EnvFileName;
            string outputFileName = "output_" + DateTime.Now.Ticks.ToString() + ".jpg";

            string facePath = Constants.LocalTempDirectory + faceFileName;
            string envPath = Constants.LocalTempDirectory + envFileName;
            string outputPath = Constants.LocalTempDirectory + outputFileName;

            cloudStorage.CreateLocalFiles(faceFileName, envFileName);

            Swapper swapper = new Swapper();
            swapper.SwapFaces(facePath, envPath, outputPath);

            cloudStorage.DeleteLocalFiles(facePath, envPath, outputPath);

            return cloudStorage.GetOutputUri(outputFileName);
        }
    }
}
