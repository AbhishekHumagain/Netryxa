using Static.Helper.Constants;
using Static.Results;
using Microsoft.AspNetCore.Http;
using Static.Application.Common.FileUpload.Dto;

namespace Static.Application.Common.Interfaces;

public interface IS3Service
{
    Task<Result> CheckIfTemplateExists(string fileKey);

    Task<string> GetPreSignedUrlAsync(StaticConstants.PreSignedUrlType preSignedUrlType,
        string subDirectoryOrSubDomain,
        string? fileName,
        bool isPdf = false,
        bool isGet = false,
        int validityInMinutes = 5);

    Task<FileUploadResponse> UploadFileToS3Async(IFormFile file,
        string subDirectoryName,
        StaticConstants.FileUploadInvokedFrom invokedFrom,
        bool isDownloadable = false,
        bool isPrivate = false,
        Guid? userId = null);
}