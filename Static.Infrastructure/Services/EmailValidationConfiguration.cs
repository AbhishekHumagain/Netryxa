using Static.Application.Common.Interfaces;

namespace Static.Infrastructure.Services;

public class EmailValidationConfiguration : IEmailValidationConfiguration
{
    public string ValidationUrl { get; set; }
}