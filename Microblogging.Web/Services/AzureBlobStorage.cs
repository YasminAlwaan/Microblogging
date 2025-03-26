using Azure.Storage.Blobs;
using Microblogging.Core.Interfaces;

namespace Microblogging.Web.Services
{
  

    public class AzureBlobStorage : IImageStorage
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureBlobStorage(IConfiguration config)
        {
            var connectionString = config["AzureStorage:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Azure Storage connection string is not configured");
            }

            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = config["AzureStorage:ContainerName"] ?? "posts";
        }

        public async Task<string> StoreImage(Stream imageStream, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(imageStream, true);

            return blobClient.Uri.ToString();
        }
    }
}
