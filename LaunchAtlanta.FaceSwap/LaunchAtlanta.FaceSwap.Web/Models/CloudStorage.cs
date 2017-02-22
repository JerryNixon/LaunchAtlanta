using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LaunchAtlanta.FaceSwap.Web.Models
{
    public class CloudStorage
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudBlobClient blobClient;
        private CloudBlobContainer faceContainer;
        CloudBlobContainer envContainer;
        CloudBlobContainer outputContainer;

        public CloudStorage()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            blobClient = storageAccount.CreateCloudBlobClient();

            faceContainer = blobClient.GetContainerReference("face");
            envContainer = blobClient.GetContainerReference("env");
            outputContainer = blobClient.GetContainerReference("output");

            CreateContainers();
        }

        public void CreateLocalFiles(string faceFileName, string envFileName)
        {
            CloudBlob face = faceContainer.GetBlobReference(faceFileName);
            CloudBlob env = envContainer.GetBlobReference(envFileName);

            string faceFilePath = Constants.LocalTempDirectory + faceFileName;
            string envFilePath = Constants.LocalTempDirectory + envFileName;

            if (!Directory.Exists(Constants.LocalTempDirectory))
                Directory.CreateDirectory(Constants.LocalTempDirectory);

            using (var faceStream = File.OpenWrite(faceFilePath))
            {
                face.DownloadToStream(faceStream);
            }

            using (var envStream = File.OpenWrite(envFilePath))
            {
                env.DownloadToStream(envStream);
            }
        }

        public void DeleteLocalFiles(string faceFilePath, string envFilePath, string outputFilePath)
        {
            File.Delete(faceFilePath);
            File.Delete(envFilePath);
            File.Delete(outputFilePath);
        }

        public string GetOutputUri(string outputFileName)
        {
            string result = string.Empty;

            CloudBlob outputFile = outputContainer.GetBlobReference(outputFileName);
            result = outputFile.Uri.ToString();

            return result;
        }

        private void CreateContainers()
        {
            var faceCreated = faceContainer.CreateIfNotExists();
            var envCreated = envContainer.CreateIfNotExists();
            var outputCreated = outputContainer.CreateIfNotExists();

            //if (faceCreated)
            //    faceContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            //if (envCreated)
            //    envContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            if (outputCreated)
                outputContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }
    }
}