using Static.Domain.Errors;
using Static.Helper.Constants;
using Static.MediatR.Command;
using Static.Results;
using Static.UserSession.CurrentUser.Guid;
using Microsoft.AspNetCore.Http;
using Static.Application.Common.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace Static.Application.Common.FileUpload.ImageUploadCommand
{
    public record ImageUploadCommand(IFormFile File) : ICommand<string>;

    public class ImageUploadCommandHandler(
        IS3Service s3Service,
        ICurrentUserService currentUserService,
        IAwsConfiguration awsConfiguration)
        : ICommandHandler<ImageUploadCommand, string>
    {
        // private readonly ILogger _logger;

        public async Task<Result<string>> Handle(ImageUploadCommand request, CancellationToken cancellationToken)
        {
            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            var contentType = request.File.ContentType.ToLowerInvariant();

            var fileTypes = new[] { ".svg", ".png", ".jpg", ".jpeg" };
            var validContentTypes = new[] { "image/jpeg", "image/png", "image/svg+xml" };
            if (!fileTypes.Contains(extension))
            {
                return HttpContextError.BadRequest("Invalid image file type.");
            }

            if (!validContentTypes.Contains(contentType))
            {
                return HttpContextError.BadRequest("Invalid content type.");
            }

            // copy the content of uploaded file to the memory stream
            // to read bytes for identifying the content of the uploaded file from the stream!! 
            // implemented to validate the integrity of the uploaded content

            var result = new StringBuilder();
            using (var reader = new StreamReader(request.File.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(await reader.ReadLineAsync(cancellationToken));
            }

            const string pattern = "script |<script|onload|onerror|javascript|src=";
            var matches = Regex.Matches(result.ToString(), pattern,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (matches.Count > 0)
            {
                return HttpContextError.BadRequest("Invalid image file content.");
            }

            var subDirectoryName = awsConfiguration.ImagesFolder;
            var userId = currentUserService.UserId;
            var uploadResponse = await s3Service.UploadFileToS3Async(request.File, subDirectoryName,
                StaticConstants.FileUploadInvokedFrom.ImageFile, false, false, userId);
            if (!uploadResponse.Success)
            {
                return HttpContextError.InternalServerError(StaticConstants.FileUploadFailedMessage);
            }

            var fileUrl = $@"/{subDirectoryName}/{uploadResponse.FileName}";
            // _logger.LogInformation("Image is uploaded at: {0}",
            //     fileUrl);

            return fileUrl;
        }
    }
}