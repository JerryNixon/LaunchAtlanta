using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchApp.Services.BlobService
{
    public class ContainerLogic
    {
        private AzureBlobHelper Parent { get; set; }
        public ContainerLogic(AzureBlobHelper parent)
        {
            this.Parent = parent;
        }

        public async Task<Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer> GetAsync(string containerName)
        {
            var container = this.Parent.GetContainer(containerName);
            try { await container.CreateIfNotExistsAsync(Microsoft.WindowsAzure.Storage.Blob.BlobContainerPublicAccessType.Container, new Microsoft.WindowsAzure.Storage.Blob.BlobRequestOptions { }, new Microsoft.WindowsAzure.Storage.OperationContext { }); }
            catch { System.Diagnostics.Debugger.Break(); }
            return container;
        }

        public async Task<bool> ExistsAsync(string containerName)
        {
            var container = this.Parent.GetContainer(containerName);
            return await container.ExistsAsync();
        }
        public async Task DeleteAsync(string containerName)
        {
            var container = this.Parent.GetContainer(containerName);
            await container.DeleteIfExistsAsync();
        }

        public Uri GetSas(Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPermissions permission, TimeSpan expiry, string containerName)
        {
            var policy = new Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPolicy
            {
                Permissions = permission,
                SharedAccessExpiryTime = DateTime.UtcNow.Add(expiry)
            };
            var container = this.Parent.GetContainer(containerName);
            var signature = container.GetSharedAccessSignature(policy);
            return new Uri(System.IO.Path.Combine(container.Uri.ToString(), signature));
        }

        public async Task<IEnumerable<Microsoft.WindowsAzure.Storage.Blob.IListBlobItem>> ListBlobsAsync(string containerName)
        {
            var container = this.Parent.GetContainer(containerName);
            var results = new List<Microsoft.WindowsAzure.Storage.Blob.IListBlobItem>();
            var continuationToken = default(Microsoft.WindowsAzure.Storage.Blob.BlobContinuationToken);
            do
            {
                var response = await container.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            } while (continuationToken != null);
            return results;
        }
    }
}
