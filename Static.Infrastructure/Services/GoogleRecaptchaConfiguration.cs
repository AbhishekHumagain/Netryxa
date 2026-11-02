using Static.Application.Common.Interfaces;

namespace Static.Infrastructure.Services;

public class GoogleRecaptchaConfiguration : IGoogleRecaptchaConfiguration
{
    public string RecaptchaPublicKey { get; set; }
    public string SecretKey { get; set; }
}