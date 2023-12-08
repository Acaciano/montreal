using Montreal.Core.Domain.Interfaces.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Montreal.Core.Crosscutting.Infrastructure.Storage
{
    public class AzureStorage : IStorage
    {
        private readonly string _connectionString;

        public AzureStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        private CloudBlockBlob GetContainer(string containerName, string fileName, bool isPrivate)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync();

            if (!isPrivate)
            {
                container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            CloudBlockBlob block = container.GetBlockBlobReference(fileName);

            return block;
        }

        public string GetBlobSasUriWithPolicy(CloudBlockBlob blob, string policyName)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List;

            var sasBlobToken = blob.GetSharedAccessSignature(sasConstraints, policyName);

            return sasBlobToken;
        }

        public void UploadFile(Stream stream, string container, string filename, bool isPrivate = false)
        {
            if (stream == null)
                throw new ArgumentNullException("stream", "Argument is null");

            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename", "Argument is null or empty");

            stream.Position = 0;

            CloudBlockBlob blob = GetContainer(container, filename, isPrivate);

            Task.WaitAll(blob.UploadFromStreamAsync(stream));
        }

        public Stream GetFile(string container, string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename", "Argument is null or empty");

            CloudBlockBlob blob = GetContainer(container, filename, false);

            var memoryStream = new MemoryStream();

            blob.DownloadToStreamAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        public string GetFileUrl(string container, string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename", "Argument is null or empty");

            CloudBlockBlob blob = GetContainer(container, filename, false);

            if(blob == null) throw new ArgumentNullException("blob", "Blob is null or empty");

            return blob.Uri.AbsoluteUri;
        }
    }
}
