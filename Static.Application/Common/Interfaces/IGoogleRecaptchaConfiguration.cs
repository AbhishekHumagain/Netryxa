namespace Static.Application.Common.Interfaces;

public interface IGoogleRecaptchaConfiguration
{
    string RecaptchaPublicKey { get; set; }
    string SecretKey { get; set; }
}