using Static.Domain.Common;
using Static.Domain.Enums;

namespace Static.Domain.Entities.Srs;

/// <summary>
/// A reusable SRS document template that defines the structure (sections) for a methodology.
/// </summary>
public class SrsTemplate : BaseAuditableEntity
{
    /// <summary>Display name of the template.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Short description of what this template is for.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Software development methodology this template is designed for.</summary>
    public SrsMethodology Methodology { get; set; }

    /// <summary>Whether this template is visible and selectable by end users.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Ordered list of section definitions that make up this template.</summary>
    public List<SrsTemplateSectionDefinition> SectionDefinitions { get; set; } = [];
}
