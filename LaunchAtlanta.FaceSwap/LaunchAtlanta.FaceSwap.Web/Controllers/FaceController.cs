using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using LaunchAtlanta.FaceSwap;

namespace LaunchAtlanta.FaceSwap.Web.Controllers
{
    public class FaceController : ApiController
    {
        // GET: api/Face
        public string Get()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer faceContainer = blobClient.GetContainerReference("face");
            CloudBlobContainer envContainer = blobClient.GetContainerReference("env");
            CloudBlobContainer outputContainer = blobClient.GetContainerReference("output");

            var faceCreated = faceContainer.CreateIfNotExists();
            var envCreated = envContainer.CreateIfNotExists();
            var outputCreated = outputContainer.CreateIfNotExists();

            if (faceCreated)
                faceContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            if (envCreated)
                envContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            if (outputCreated)
                outputContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            CloudBlob face = faceContainer.GetBlobReference("donald.jpg");
            CloudBlob env = envContainer.GetBlobReference("hillary.jpg");

            using (var faceStream = File.OpenWrite(@"c:\tmp\donald.jpg"))
            {
                face.DownloadToStream(faceStream);
            }

            using (var envStream = File.OpenWrite(@"c:\tmp\hillary.jpg"))
            {
                env.DownloadToStream(envStream);
            }

            Swapper swapper = new Swapper();
            var result = swapper.SwapFaces(@"c:\tmp\donald.jpg", @"c:\tmp\hillary.jpg", @"c:\tmp\output.jpg");

            return result;
        }

        // GET: api/Face/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Face
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Face/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Face/5
        public void Delete(int id)
        {
        }
    }
}
