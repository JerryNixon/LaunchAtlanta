using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LaunchApp.Servicez.BlobService
{
    class BlobService
    {
        string _containerName;
        AzureBlobHelper _helper = default(AzureBlobHelper);
        public BlobService(string blobAccountName, string blobAccessKey, string transferGroupName, string blobContainerName)
        {
            _containerName = blobContainerName;
            _helper = new AzureBlobHelper(blobAccountName, blobAccessKey, transferGroupName);
        }

        public Uri GetReadPath(string name)
        {
            var expire = TimeSpan.FromDays(365 * 2);
            var read = Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPermissions.Read;
            return _helper.Item.GetSas(read, expire, _containerName, name);
        }

        public async Task UploadAsync(StorageFile source, string name = null)
        {
            name = name ?? source.Name;
            await _helper.Item.UploadAsync(source, _containerName, name);
        }
    }
}
