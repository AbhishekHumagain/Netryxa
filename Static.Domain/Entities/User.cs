using Static.Helper.Constants;
using Static.Helper.Constants.EnumCollections;
using Static.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Static.Domain.Entities;

public class User : BaseAuditableEntity
{
    public const int NickNameLength = 50;
    
    public string FullName { get; set; }
    public string EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    [StringLength(NickNameLength)]
    public string? NickName { get; set; }
    public string? PhotoUrl { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public StaticConstants.Localization Localization { get; set; } = StaticConstants.Localization.English;
    public StaticRole Role { get; set; }
    public StaticConstants.Status Status { get; set; } = StaticConstants.Status.Active;
    public StaticConstants.UserMode UserMode { get; set; } = StaticConstants.UserMode.FullVersion;
    public bool ForcePasswordUpdate { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsPushNotificationEnabled {  get; set; }
    public bool IsEmailNotificationEnabled {  get; set; }
}