using Azure.Storage.Blobs;

namespace Microblogging.Web.Services
{
    public interface IImageStorage
    {
        Task<string> StoreImage(Stream imageStream, string fileName);
    }

    public class AzureBlobStorage : IImageStorage
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureBlobStorage(IConfiguration config)
        {
            _blobServiceClient = new BlobServiceClient(
                config["AzureStorage:ConnectionString"]);
            _containerName = config["AzureStorage:ContainerName"];
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
