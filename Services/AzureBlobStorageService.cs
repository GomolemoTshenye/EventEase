using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

public class AzureBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureBlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "eventease-images";

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString),
                "Azure Blob Storage connection string is not configured");
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadFileAsync(byte[] fileData, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(fileName);
        using var stream = new MemoryStream(fileData);
        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobName = Path.GetFileName(new Uri(fileUrl).LocalPath);
        await containerClient.DeleteBlobIfExistsAsync(blobName);
    }
}
