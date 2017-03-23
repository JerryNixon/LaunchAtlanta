using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchApp.Services.BlobService
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AzureBlobHelper
    {
        public Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient CloudBlobClient { get; private set; }

        public AzureBlobHelper(string accountName, string accessKey, string groupName, bool useHttps = false)
        {
            var credentials = new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(accountName, accessKey);
            var storageAccount = new Microsoft.WindowsAzure.Storage.CloudStorageAccount(credentials, useHttps);
            CloudBlobClient = storageAccount.CreateCloudBlobClient();
            Container = new ContainerLogic(this);
            Item = new ItemLogic(this, groupName);
        }

        internal Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer GetContainer(string containerName)
        {
            // cannot be empty
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentNullException("containerName cannot be empty");
       
            // must all be lower case
            if (containerName != containerName.ToLower())
                throw new ArgumentNullException("containerName must be lower case");
        
            // first letter must be number or letter
            if (!System.Text.RegularExpressions.Regex.IsMatch(containerName, @"[\w\d]"))
                throw new ArgumentNullException("containerName must start with a number or letter");
          
            // only letter, hyphen, number
            if (!System.Text.RegularExpressions.Regex.IsMatch(containerName, @"^[-\w\d]"))
                throw new ArgumentNullException("containerName must only contain numbers, letters, and hyphens");

            return CloudBlobClient.GetContainerReference(containerName);
        }

        public ContainerLogic Container { get; private set; }

        public ItemLogic Item { get; private set; }
    }
}
