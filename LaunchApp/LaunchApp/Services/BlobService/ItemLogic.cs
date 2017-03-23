using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchApp.Services.BlobService
{
    public class ItemLogic
    {
        private AzureBlobHelper Parent { get; set; }
        private string _groupName;
        public ItemLogic(AzureBlobHelper parent, string groupName)
        {
            this.Parent = parent;
            this._groupName = groupName;
        }

        /*
            Read
            Container: Not applicable on blob container.
            Blob: Gives the ability to read the contents of the blob. Also gives the ability to read the metadata and properties of the blob as well.

            Write
            Container: Gives the ability to upload one or more blobs in blob container. Also gives the ability to update properties, metadata and create snapshots of a blob.
            Blob: Give the ability to upload a new blob (by the same name) or overwrite an existing blob (by the same name). Also gives the ability to update properties, metadata and create snapshots of a blob.

            List
            Container: Lists the blobs in the blob container.
            Blob: Not applicable on blobs.

            Delete
            Container: Not applicable on blob container.
            Blob: Gives the ability to delete the blob.
        */

        public Uri GetSas(Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPermissions permission, TimeSpan expiry, string containerName, string blobName)
        {
            var policy = new Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPolicy
            {
                Permissions = permission,
                SharedAccessExpiryTime = null
            };
            var container = this.Parent.GetContainer(containerName);
            var signature = container.GetSharedAccessSignature(policy);
            var blob = container.GetBlockBlobReference(blobName);
            return new Uri(System.IO.Path.Combine(blob.Uri.ToString(), signature));
        }

        public async Task UploadAsync(Windows.Storage.StorageFile sourceFile,
            string containerName, string blobName)
        {
            var properties = await sourceFile.GetBasicPropertiesAsync();
            if (properties.Size > (1024 * 1024 * 64))
                throw new Exception("File cannot be larger than 64MB");

            var container = this.Parent.GetContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);
            await blob.UploadFromFileAsync(sourceFile);
        }

        public async Task<Windows.Networking.BackgroundTransfer.UploadOperation> UploadBackgroundAsync(Windows.Storage.StorageFile sourceFile,
            string containerName, string blobName, Action<Windows.Networking.BackgroundTransfer.UploadOperation> progressCallback = null)
        {
            var properties = await sourceFile.GetBasicPropertiesAsync();
            if (properties.Size > (1024 * 1024 * 64))
                throw new Exception("File cannot be larger than 64MB");

            var permissions = Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPermissions.Write;
            var container = this.Parent.GetContainer(containerName);
            var signature = this.GetSas(permissions, TimeSpan.FromMinutes(5), container.Name, blobName);
            var uploader = new Windows.Networking.BackgroundTransfer.BackgroundUploader
            {
                TransferGroup = Windows.Networking.BackgroundTransfer.BackgroundTransferGroup.CreateGroup(_groupName)
            };
            uploader.SetRequestHeader("Filename", sourceFile.Name);
            var upload = uploader.CreateUpload(signature, sourceFile);
            if (progressCallback == null)
                return await upload.StartAsync();
            else
                return await upload.StartAsync().AsTask(new Progress<Windows.Networking.BackgroundTransfer.UploadOperation>(progressCallback));
        }

        public async Task DownloadAsync(Windows.Storage.StorageFile targetFile,
            string containerName, string blobName)
        {
            var container = this.Parent.GetContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);
            await blob.DownloadToFileAsync(targetFile);
        }

        public async Task<Windows.Networking.BackgroundTransfer.DownloadOperation> DownloadBackgroundAsync(Windows.Storage.StorageFile targetFile,
            string containerName, string blobName, Action<Windows.Networking.BackgroundTransfer.DownloadOperation> progressCallback = null)
        {
            var read = Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPermissions.Read;
            var container = this.Parent.GetContainer(containerName);
            var signature = this.GetSas(read, TimeSpan.FromMinutes(5), container.Name, blobName);
            var downloader = new Windows.Networking.BackgroundTransfer.BackgroundDownloader
            {
                TransferGroup = Windows.Networking.BackgroundTransfer.BackgroundTransferGroup.CreateGroup(_groupName)
            };
            var download = await downloader.CreateDownloadAsync(signature, targetFile, null);
            if (progressCallback == null)
                return await download.StartAsync();
            else
                return await download.StartAsync().AsTask(new Progress<Windows.Networking.BackgroundTransfer.DownloadOperation>(progressCallback));
        }

        public async Task<bool> ExistsAsync(string containerName, string blobName)
        {
            var container = this.Parent.GetContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);
            return await blob.ExistsAsync();
        }

        public async Task DeleteAsync(string containerName, string blobName)
        {
            var container = this.Parent.GetContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);
            await blob.DeleteIfExistsAsync();
        }

        public async Task CopyAsync(string containerName, string blobName, string newName)
        {
            var container = this.Parent.GetContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);
            var newBlob = container.GetBlockBlobReference(newName);
            await blob.StartCopyAsync(blob);
        }
    }

}
