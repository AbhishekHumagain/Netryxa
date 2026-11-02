using Amazon.S3;
using Amazon.S3.Model;
using Static.Application.Common;
using Static.Application.Common.FileUpload.Dto;
using Static.Application.Common.Interfaces;
using Static.Application.Common.Interfaces.Shared;
using Static.Domain.Errors;
using Static.Helper.Constants;
using Static.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Static.Infrastructure.Services.AmazonService.S3;

public class S3Service : IS3Service
{
    private readonly IConfiguration _configuration;
    private readonly IAmazonS3 _defaultS3Client;
    private readonly IS3ClientFactory _s3ClientFactory;
    private readonly IAwsConfiguration _awsConfiguration;

    public S3Service(IConfiguration configuration,
        IAwsConfiguration awsConfiguration,
        IS3ClientFactory s3ClientFactory)
    {
        _configuration = configuration;
        _awsConfiguration = awsConfiguration;
        _s3ClientFactory = s3ClientFactory;
        // Caching default client so that,factory shouldn't be called every time
        _defaultS3Client = s3ClientFactory.GetDefaultClient();
    }

    #region Special Launch Page

    public async Task<Result> CheckIfTemplateExists(string fileKey)
    {
        var bucketName = $@"{_awsConfiguration.AssetsS3BucketName}";
        fileKey = $@"{_awsConfiguration.SpecialLaunchPageAttachmentsBucketFolder}/{fileKey}";
        try
        {
            var response = await _defaultS3Client.GetObjectMetadataAsync(bucketName, fileKey);
            if (response is { ETag: not null })
            {
                return Result.Success();
            }
        }
        catch (AmazonS3Exception ex)
        {
            return HttpContextError.Ambiguous($"S3 Client Error:{ex.Message}");
        }

        return Result.Success();
    }

    #endregion

    public async Task<string> GetPreSignedUrlAsync(StaticConstants.PreSignedUrlType preSignedUrlType,
        string subDirectoryOrSubDomain,
        string? fileName,
        bool isPdf = false,
        bool isGet = false,
        int validityInMinutes = 5)
    {
        IAmazonS3 client;
        GetPreSignedUrlRequest request;
        var verbType = isGet ? HttpVerb.GET : HttpVerb.PUT;
        var zoneName = _configuration["CloudFlare:ZoneName"];
        var attachmentsBucket = _awsConfiguration.AttachmentsBucket;
        var assetsBucket = _awsConfiguration.AssetsS3BucketName;
        switch (preSignedUrlType)
        {
            case StaticConstants.PreSignedUrlType.Classroom:
                client = _s3ClientFactory.GetClientForRegion(StaticConstants.AwsRegionClassroomBucketEndPoint);
                request = new GetPreSignedUrlRequest
                {
                    BucketName = $"{attachmentsBucket}",
                    Key = $"{subDirectoryOrSubDomain}/{fileName}",
                    Verb = verbType,
                    Expires = DateTime.UtcNow.AddMinutes(validityInMinutes),
                };
                return await client.GetPreSignedURLAsync(request);
                break;

            case StaticConstants.PreSignedUrlType.SpecialLaunchPage:
                client = _s3ClientFactory.GetClientForRegion(StaticConstants.AwsRegionSpecialLaunchPageBucketEndpoint);
                request = new GetPreSignedUrlRequest
                {
                    BucketName = attachmentsBucket,
                    Key = $@"{subDirectoryOrSubDomain}.{zoneName}/index.html",
                    Verb = verbType,
                    Expires = DateTime.UtcNow.AddMinutes(validityInMinutes),
                };
                return await client.GetPreSignedURLAsync(request);
                break;

            default:
                request = new GetPreSignedUrlRequest
                {
                    BucketName = $"{assetsBucket}",
                    Key = $"{subDirectoryOrSubDomain}/{fileName}",
                    Verb = verbType,
                    ContentType = isPdf ? "application/pdf" : string.Empty,
                    Expires = DateTime.UtcNow.AddMinutes(validityInMinutes),
                };
                return await _defaultS3Client.GetPreSignedURLAsync(request);
                break;
        }
    }

    public async Task<FileUploadResponse> UploadFileToS3Async(IFormFile file,
        string subDirectoryName,
        StaticConstants.FileUploadInvokedFrom invokedFrom,
        bool isDownloadable = false,
        bool isPrivate = false,
        Guid? userId = null)
    {
        if (file.Length == 0)
        {
            return new FileUploadResponse { Success = false, Message = "The uploaded file is empty." };
        }

        var fileBytes = new byte[file.Length];
        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!Array.Exists(StaticConstants.AllowedUploadFileType, ext => ext.Equals(fileExtension)))
        {
            return new FileUploadResponse { Success = false, Message = $"File type {fileExtension} is not allowed!" };
        }

        var cleanFileName = CustomRegex.CleanFileNameRegex().Replace(file.FileName, "").Replace(fileExtension, "");
        cleanFileName = string.IsNullOrEmpty(cleanFileName) ? "Default" : cleanFileName.Replace(" ", "_");
        var identifier = userId.ToString() ?? userId?.ToString() ?? Guid.NewGuid().ToString();
        var fileName = $"{cleanFileName}_{Guid.NewGuid()}_{identifier}_{fileExtension}";
        var contentType = isDownloadable ? "binary/octet-stream" : file.ContentType;
        var bucketName = _awsConfiguration.AssetsS3BucketName;
        string s3Key;
        switch (invokedFrom)
        {
            case StaticConstants.FileUploadInvokedFrom.SpecialLaunchPage:
                var specialLaunchPageFolder = _awsConfiguration.SpecialLaunchPageAttachmentsBucketFolder;
                s3Key = $"{specialLaunchPageFolder}/{subDirectoryName}/{fileName}";
                break;

            default:
                s3Key = $"{subDirectoryName}/{fileName}";
                break;
        }

        using (var stream = new MemoryStream(fileBytes))
        {
            var request = new PutObjectRequest
            {
                BucketName = $"{bucketName}".TrimEnd('/'),
                Key = s3Key,
                InputStream = stream,
                ContentType = contentType,
                CannedACL = isPrivate ? S3CannedACL.Private : S3CannedACL.BucketOwnerFullControl,
            };

            var response = await _defaultS3Client.PutObjectAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return new FileUploadResponse
                {
                    Success = true, Message = "File successfully uploaded.", FileName = fileName
                };
            }
        }

        return new FileUploadResponse { Success = false, Message = "File upload failed!", FileName = fileName };
    }
}