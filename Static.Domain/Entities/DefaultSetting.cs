using Static.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Static.Domain.Entities;

public class DefaultSetting : BaseAuditableEntity
{
    [Required]
    public string Name { get; set; }
    public string Value { get; set; }
    
    public bool IsJson { get; set; }
}