using System.ComponentModel.DataAnnotations;

namespace Static.API.ValidationAttributes;

/// <summary>
/// Specifies that a file must have an allowed extension.
/// </summary>
/// <param name="extensions">The allowed file extensions.</param>
public class AllowedFileExtensionsAttribute(params string[] extensions) : ValidationAttribute
{
    /// <summary>
    /// Validates whether the uploaded file has an allowed extension.
    /// </summary>
    /// <param name="value">The object to validate.</param>
    /// <param name="validationContext">The context in which validation is performed.</param>
    /// <returns>
    /// A <see cref="ValidationResult"/> indicating whether the file's extension is valid.
    /// </returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IFormFile file) return ValidationResult.Success;

        var extension = Path.GetExtension(file.FileName);
        return extensions.Contains(extension, StringComparer.OrdinalIgnoreCase)
            ? ValidationResult.Success
            : new(
                $"The file extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", extensions)}.",
                [validationContext.MemberName ?? validationContext.DisplayName]);
    }
}