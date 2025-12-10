using Amazon.S3;
using Amazon.S3.Transfer;
using CatalogHub.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace CatalogHub.Application.Services;

public class AWSStorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public AWSStorageService(IAmazonS3 s3Client, IConfiguration config)
    {
        _s3Client = s3Client;
        _bucketName = config["AWS:BucketName"];
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType)
    {
        var key = $"products/{Guid.NewGuid()}_{fileName}";
        var transferUtility = new TransferUtility(_s3Client);

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = fileStream,
            Key = key,
            BucketName = _bucketName,
            ContentType = contentType ?? "application/octet-stream"
        };

        await transferUtility.UploadAsync(uploadRequest);
        return $"https://{_bucketName}.s3.amazonaws.com/{key}";
    }
}
