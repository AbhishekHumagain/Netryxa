using Static.EntityFrameworkCore.Contracts.Auditing;
using Static.EntityFrameworkCore.Contracts.Entity;
using Static.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Static.Domain.Entities;

public class UserLoginAttempt : IEntity, IHasCreationTime
{
    public const int MaxUserNameOrEmailAddressLength = 256;
    public const int MaxClientIpAddressLength = 64;
    public const int MaxBrowserInfoLength = 512;

    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public virtual User User { get; set; }

    [StringLength(MaxUserNameOrEmailAddressLength)]
    public string UserNameOrEmailAddress { get; set; }

    [StringLength(MaxClientIpAddressLength)]
    public string ClientIpAddress { get; set; }

    [StringLength(MaxBrowserInfoLength)] public string BrowserInfo { get; set; }
    public LoginResultType Result { get; set; }
    public DateTime CreationTime { get; set; }
}