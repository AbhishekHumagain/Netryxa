using FluentValidation;

namespace Static.Application.Common.FileUpload.ImageUploadCommand
{
    public class ImageUploadCommandValidator : AbstractValidator<ImageUploadCommand>
    {
        public ImageUploadCommandValidator()
        {
            RuleFor(x => x.File.Length).NotNull().LessThan(1048576) 
                .WithMessage("File size is larger than 1MB is not allowed.");
            RuleFor(x => x.File.Length).NotNull().GreaterThan(0) 
                .WithMessage("File size must be larger then 0MB.");
            RuleFor(x => x.File.ContentType).NotNull().Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"))
                .WithMessage("File type is not valid.");
        }
    }
}
