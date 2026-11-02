using Microsoft.EntityFrameworkCore;
using Static.Domain.Entities.Wbs;
using Static.Domain.Enums;

namespace Static.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds built-in WBS templates for each supported development methodology.
/// Each template contains a pre-built node tree that mirrors real-world WBS practice.
///
/// Node colours follow a consistent semantic palette:
///   Project   #4F46E5  indigo
///   Phase     #0EA5E9  sky
///   Summary   #8B5CF6  violet
///   Deliverable #10B981 emerald
///   WorkPackage #F59E0B amber
///   Milestone #EF4444  red
/// </summary>
public sealed class WbsTemplateSeeder(StaticDbContext db) : ISeeder
{
    public async Task SeedAsync()
    {
        if (await db.WbsTemplate.AnyAsync())
            return;

        var templates = new List<WbsTemplate>
        {
            BuildWaterfallTemplate(),
            BuildAgileTemplate(),
            BuildScrumTemplate(),
            BuildRupTemplate(),
            BuildCustomTemplate()
        };

        await db.WbsTemplate.AddRangeAsync(templates);
        await db.SaveChangesAsync();
    }

    // ── Colours ───────────────────────────────────────────────────────────────
    private const string CProject     = "#4F46E5";
    private const string CPhase       = "#0EA5E9";
    private const string CSummary     = "#8B5CF6";
    private const string CDeliverable = "#10B981";
    private const string CWork        = "#F59E0B";
    private const string CMilestone   = "#EF4444";

    // ── 1. Waterfall (IEEE 830 inspired) ─────────────────────────────────────
    private static WbsTemplate BuildWaterfallTemplate()
    {
        // Pre-assign all IDs so parent references are accurate
        var root  = Guid.NewGuid();
        var p11   = Guid.NewGuid(); var p12 = Guid.NewGuid(); var p13 = Guid.NewGuid();
        var p14   = Guid.NewGuid(); var p15 = Guid.NewGuid(); var p16 = Guid.NewGuid();
        var p17   = Guid.NewGuid();

        return new WbsTemplate
        {
            Id          = Guid.NewGuid(),
            Name        = "Waterfall WBS",
            Description = "Classic phase-gate WBS following the Waterfall model — Initiation through Closure.",
            Methodology = WbsMethodology.Waterfall,
            IsActive    = true,
            Nodes       = new List<WbsTemplateNode>
            {
                // Root
                N(root, null,  "1.0", "Project",     WbsNodeType.Project,     0, 0, CProject),

                // Phases
                N(p11, root,   "1.1", "Initiation",  WbsNodeType.Phase,       1, 1, CPhase),
                N(p12, root,   "1.2", "Planning",    WbsNodeType.Phase,       2, 1, CPhase),
                N(p13, root,   "1.3", "Design",      WbsNodeType.Phase,       3, 1, CPhase),
                N(p14, root,   "1.4", "Development", WbsNodeType.Phase,       4, 1, CPhase),
                N(p15, root,   "1.5", "Testing",     WbsNodeType.Phase,       5, 1, CPhase),
                N(p16, root,   "1.6", "Deployment",  WbsNodeType.Phase,       6, 1, CPhase),
                N(p17, root,   "1.7", "Closure",     WbsNodeType.Phase,       7, 1, CPhase),

                // 1.1 Initiation
                N(Guid.NewGuid(), p11, "1.1.1", "Define Scope",          WbsNodeType.WorkPackage,  1, 2, CWork),
                N(Guid.NewGuid(), p11, "1.1.2", "Identify Stakeholders", WbsNodeType.WorkPackage,  2, 2, CWork),
                N(Guid.NewGuid(), p11, "1.1.3", "Project Charter",       WbsNodeType.Deliverable,  3, 2, CDeliverable),

                // 1.2 Planning
                N(Guid.NewGuid(), p12, "1.2.1", "Requirements Analysis", WbsNodeType.WorkPackage,  1, 2, CWork),
                N(Guid.NewGuid(), p12, "1.2.2", "System Design",         WbsNodeType.WorkPackage,  2, 2, CWork),
                N(Guid.NewGuid(), p12, "1.2.3", "Project Plan",          WbsNodeType.Deliverable,  3, 2, CDeliverable),
                N(Guid.NewGuid(), p12, "1.2.4", "Risk Assessment",       WbsNodeType.WorkPackage,  4, 2, CWork),

                // 1.3 Design
                N(Guid.NewGuid(), p13, "1.3.1", "Architecture Design",   WbsNodeType.Deliverable,  1, 2, CDeliverable),
                N(Guid.NewGuid(), p13, "1.3.2", "UI/UX Design",          WbsNodeType.Deliverable,  2, 2, CDeliverable),
                N(Guid.NewGuid(), p13, "1.3.3", "Database Design",       WbsNodeType.Deliverable,  3, 2, CDeliverable),

                // 1.4 Development
                N(Guid.NewGuid(), p14, "1.4.1", "Backend Development",   WbsNodeType.WorkPackage,  1, 2, CWork),
                N(Guid.NewGuid(), p14, "1.4.2", "Frontend Development",  WbsNodeType.WorkPackage,  2, 2, CWork),
                N(Guid.NewGuid(), p14, "1.4.3", "Integration",           WbsNodeType.WorkPackage,  3, 2, CWork),

                // 1.5 Testing
                N(Guid.NewGuid(), p15, "1.5.1", "Unit Testing",          WbsNodeType.WorkPackage,  1, 2, CWork),
                N(Guid.NewGuid(), p15, "1.5.2", "Integration Testing",   WbsNodeType.WorkPackage,  2, 2, CWork),
                N(Guid.NewGuid(), p15, "1.5.3", "User Acceptance Testing",WbsNodeType.Milestone,   3, 2, CMilestone),

                // 1.6 Deployment
                N(Guid.NewGuid(), p16, "1.6.1", "Environment Setup",     WbsNodeType.WorkPackage,  1, 2, CWork),
                N(Guid.NewGuid(), p16, "1.6.2", "Data Migration",        WbsNodeType.WorkPackage,  2, 2, CWork),
                N(Guid.NewGuid(), p16, "1.6.3", "Go-Live",               WbsNodeType.Milestone,    3, 2, CMilestone),

                // 1.7 Closure
                N(Guid.NewGuid(), p17, "1.7.1", "Documentation",         WbsNodeType.Deliverable,  1, 2, CDeliverable),
                N(Guid.NewGuid(), p17, "1.7.2", "Project Review",        WbsNodeType.WorkPackage,  2, 2, CWork),
                N(Guid.NewGuid(), p17, "1.7.3", "Handover",              WbsNodeType.Milestone,    3, 2, CMilestone),
            }
        };
    }

    // ── 2. Agile ──────────────────────────────────────────────────────────────
    private static WbsTemplate BuildAgileTemplate()
    {
        var root = Guid.NewGuid();
        var p11  = Guid.NewGuid(); var p12 = Guid.NewGuid(); var p13 = Guid.NewGuid();
        var p14  = Guid.NewGuid(); var p15 = Guid.NewGuid();

        return new WbsTemplate
        {
            Id          = Guid.NewGuid(),
            Name        = "Agile WBS",
            Description = "Epic-driven WBS for Agile teams — product foundation, feature epics, and release management.",
            Methodology = WbsMethodology.Agile,
            IsActive    = true,
            Nodes       = new List<WbsTemplateNode>
            {
                N(root, null,  "1.0", "Product",              WbsNodeType.Project,     0, 0, CProject),

                N(p11, root,   "1.1", "Product Foundation",   WbsNodeType.Summary,     1, 1, CSummary),
                N(p12, root,   "1.2", "Epic 1: Authentication",WbsNodeType.Summary,    2, 1, CSummary),
                N(p13, root,   "1.3", "Epic 2: Core Features",WbsNodeType.Summary,     3, 1, CSummary),
                N(p14, root,   "1.4", "Epic 3: Reporting",    WbsNodeType.Summary,     4, 1, CSummary),
                N(p15, root,   "1.5", "Release Management",   WbsNodeType.Phase,       5, 1, CPhase),

                // 1.1
                N(Guid.NewGuid(), p11, "1.1.1", "Product Vision",        WbsNodeType.Deliverable, 1, 2, CDeliverable),
                N(Guid.NewGuid(), p11, "1.1.2", "Roadmap",               WbsNodeType.Deliverable, 2, 2, CDeliverable),
                N(Guid.NewGuid(), p11, "1.1.3", "Backlog Refinement",    WbsNodeType.WorkPackage, 3, 2, CWork),

                // 1.2
                N(Guid.NewGuid(), p12, "1.2.1", "User Registration",     WbsNodeType.WorkPackage, 1, 2, CWork),
                N(Guid.NewGuid(), p12, "1.2.2", "User Login",            WbsNodeType.WorkPackage, 2, 2, CWork),
                N(Guid.NewGuid(), p12, "1.2.3", "Password Reset",        WbsNodeType.WorkPackage, 3, 2, CWork),

                // 1.3
                N(Guid.NewGuid(), p13, "1.3.1", "Feature A",             WbsNodeType.WorkPackage, 1, 2, CWork),
                N(Guid.NewGuid(), p13, "1.3.2", "Feature B",             WbsNodeType.WorkPackage, 2, 2, CWork),
                N(Guid.NewGuid(), p13, "1.3.3", "Feature C",             WbsNodeType.WorkPackage, 3, 2, CWork),

                // 1.4
                N(Guid.NewGuid(), p14, "1.4.1", "Dashboard",             WbsNodeType.WorkPackage, 1, 2, CWork),
                N(Guid.NewGuid(), p14, "1.4.2", "Data Export",           WbsNodeType.WorkPackage, 2, 2, CWork),

                // 1.5
                N(Guid.NewGuid(), p15, "1.5.1", "Sprint Reviews",        WbsNodeType.WorkPackage, 1, 2, CWork),
                N(Guid.NewGuid(), p15, "1.5.2", "Retrospectives",        WbsNodeType.WorkPackage, 2, 2, CWork),
                N(Guid.NewGuid(), p15, "1.5.3", "Production Release",    WbsNodeType.Milestone,   3, 2, CMilestone),
            }
        };
    }

    // ── 3. Scrum ──────────────────────────────────────────────────────────────
    private static WbsTemplate BuildScrumTemplate()
    {
        var root = Guid.NewGuid();
        var sp0  = Guid.NewGuid(); var sp1 = Guid.NewGuid();
        var sp2  = Guid.NewGuid(); var sp3 = Guid.NewGuid();

        return new WbsTemplate
        {
            Id          = Guid.NewGuid(),
            Name        = "Scrum WBS",
            Description = "Sprint-structured WBS — Setup sprint followed by feature sprints, each ending with a review milestone.",
            Methodology = WbsMethodology.Scrum,
            IsActive    = true,
            Nodes       = new List<WbsTemplateNode>
            {
                N(root, null, "1.0", "Product",                     WbsNodeType.Project,     0, 0, CProject),

                N(sp0, root,  "1.1", "Sprint 0: Setup",             WbsNodeType.Phase,       1, 1, CPhase),
                N(sp1, root,  "1.2", "Sprint 1: Auth MVP",          WbsNodeType.Phase,       2, 1, CPhase),
                N(sp2, root,  "1.3", "Sprint 2: Core Features",     WbsNodeType.Phase,       3, 1, CPhase),
                N(sp3, root,  "1.4", "Sprint 3: Polish & Release",  WbsNodeType.Phase,       4, 1, CPhase),

                // Sprint 0
                N(Guid.NewGuid(), sp0, "1.1.1", "Dev Environment",         WbsNodeType.WorkPackage, 1, 2, CWork),
                N(Guid.NewGuid(), sp0, "1.1.2", "CI/CD Pipeline",          WbsNodeType.WorkPackage, 2, 2, CWork),
                N(Guid.NewGuid(), sp0, "1.1.3", "Architecture Spike",      WbsNodeType.WorkPackage, 3, 2, CWork),
                N(Guid.NewGuid(), sp0, "1.1.4", "Sprint 0 Review",         WbsNodeType.Milestone,   4, 2, CMilestone),

                // Sprint 1
                N(Guid.NewGuid(), sp1, "1.2.1", "US-001: User Registration",WbsNodeType.WorkPackage,1, 2, CWork),
                N(Guid.NewGuid(), sp1, "1.2.2", "US-002: User Login",       WbsNodeType.WorkPackage,2, 2, CWork),
                N(Guid.NewGuid(), sp1, "1.2.3", "US-003: Profile Page",     WbsNodeType.WorkPackage,3, 2, CWork),
                N(Guid.NewGuid(), sp1, "1.2.4", "Sprint 1 Review",          WbsNodeType.Milestone,  4, 2, CMilestone),

                // Sprint 2
                N(Guid.NewGuid(), sp2, "1.3.1", "US-004: Dashboard",        WbsNodeType.WorkPackage,1, 2, CWork),
                N(Guid.NewGuid(), sp2, "1.3.2", "US-005: Notifications",    WbsNodeType.WorkPackage,2, 2, CWork),
                N(Guid.NewGuid(), sp2, "1.3.3", "Sprint 2 Review",          WbsNodeType.Milestone,  3, 2, CMilestone),

                // Sprint 3
                N(Guid.NewGuid(), sp3, "1.4.1", "US-006: Reports",          WbsNodeType.WorkPackage,1, 2, CWork),
                N(Guid.NewGuid(), sp3, "1.4.2", "US-007: Export",           WbsNodeType.WorkPackage,2, 2, CWork),
                N(Guid.NewGuid(), sp3, "1.4.3", "Regression Testing",       WbsNodeType.WorkPackage,3, 2, CWork),
                N(Guid.NewGuid(), sp3, "1.4.4", "Production Deployment",    WbsNodeType.Milestone,  4, 2, CMilestone),
            }
        };
    }

    // ── 4. RUP ───────────────────────────────────────────────────────────────
    private static WbsTemplate BuildRupTemplate()
    {
        var root = Guid.NewGuid();
        var inc  = Guid.NewGuid(); var ela = Guid.NewGuid();
        var con  = Guid.NewGuid(); var tra = Guid.NewGuid();

        return new WbsTemplate
        {
            Id          = Guid.NewGuid(),
            Name        = "RUP WBS",
            Description = "Rational Unified Process WBS — four phases (Inception, Elaboration, Construction, Transition) each ending with a lifecycle milestone.",
            Methodology = WbsMethodology.RUP,
            IsActive    = true,
            Nodes       = new List<WbsTemplateNode>
            {
                N(root, null,  "1.0", "Project",      WbsNodeType.Project, 0, 0, CProject),

                N(inc, root,   "1.1", "Inception",    WbsNodeType.Phase,   1, 1, CPhase),
                N(ela, root,   "1.2", "Elaboration",  WbsNodeType.Phase,   2, 1, CPhase),
                N(con, root,   "1.3", "Construction", WbsNodeType.Phase,   3, 1, CPhase),
                N(tra, root,   "1.4", "Transition",   WbsNodeType.Phase,   4, 1, CPhase),

                // Inception
                N(Guid.NewGuid(), inc, "1.1.1", "Use Case Modelling",            WbsNodeType.WorkPackage,  1, 2, CWork),
                N(Guid.NewGuid(), inc, "1.1.2", "Business Case",                 WbsNodeType.Deliverable,  2, 2, CDeliverable),
                N(Guid.NewGuid(), inc, "1.1.3", "Risk Assessment",               WbsNodeType.WorkPackage,  3, 2, CWork),
                N(Guid.NewGuid(), inc, "1.1.4", "Lifecycle Objectives Milestone", WbsNodeType.Milestone,   4, 2, CMilestone),

                // Elaboration
                N(Guid.NewGuid(), ela, "1.2.1", "Architecture Baseline",          WbsNodeType.Deliverable, 1, 2, CDeliverable),
                N(Guid.NewGuid(), ela, "1.2.2", "Risk Mitigation",                WbsNodeType.WorkPackage, 2, 2, CWork),
                N(Guid.NewGuid(), ela, "1.2.3", "Prototyping",                    WbsNodeType.WorkPackage, 3, 2, CWork),
                N(Guid.NewGuid(), ela, "1.2.4", "Lifecycle Architecture Milestone",WbsNodeType.Milestone,  4, 2, CMilestone),

                // Construction
                N(Guid.NewGuid(), con, "1.3.1", "Component Development",          WbsNodeType.WorkPackage, 1, 2, CWork),
                N(Guid.NewGuid(), con, "1.3.2", "Integration & Testing",          WbsNodeType.WorkPackage, 2, 2, CWork),
                N(Guid.NewGuid(), con, "1.3.3", "User Documentation",             WbsNodeType.Deliverable, 3, 2, CDeliverable),
                N(Guid.NewGuid(), con, "1.3.4", "Initial Operational Capability", WbsNodeType.Milestone,   4, 2, CMilestone),

                // Transition
                N(Guid.NewGuid(), tra, "1.4.1", "Beta Release",                   WbsNodeType.WorkPackage, 1, 2, CWork),
                N(Guid.NewGuid(), tra, "1.4.2", "User Training",                  WbsNodeType.WorkPackage, 2, 2, CWork),
                N(Guid.NewGuid(), tra, "1.4.3", "Production Deployment",          WbsNodeType.WorkPackage, 3, 2, CWork),
                N(Guid.NewGuid(), tra, "1.4.4", "Product Release Milestone",      WbsNodeType.Milestone,   4, 2, CMilestone),
            }
        };
    }

    // ── 5. Custom (blank) ─────────────────────────────────────────────────────
    private static WbsTemplate BuildCustomTemplate()
    {
        var root = Guid.NewGuid();
        var p1   = Guid.NewGuid();
        var p2   = Guid.NewGuid();

        return new WbsTemplate
        {
            Id          = Guid.NewGuid(),
            Name        = "Custom (Blank)",
            Description = "Start with a minimal two-phase skeleton and build any hierarchy you need.",
            Methodology = WbsMethodology.Custom,
            IsActive    = true,
            Nodes       = new List<WbsTemplateNode>
            {
                N(root, null, "1.0", "Project", WbsNodeType.Project, 0, 0, CProject),
                N(p1,   root, "1.1", "Phase 1", WbsNodeType.Phase,   1, 1, CPhase),
                N(p2,   root, "1.2", "Phase 2", WbsNodeType.Phase,   2, 1, CPhase),
            }
        };
    }

    // ── Node factory helper ───────────────────────────────────────────────────
    private static WbsTemplateNode N(
        Guid        id,
        Guid?       parentId,
        string      wbsCode,
        string      title,
        WbsNodeType nodeType,
        int         orderIndex,
        int         level,
        string      color) => new()
    {
        Id           = id,
        ParentNodeId = parentId,
        WbsCode      = wbsCode,
        Title        = title,
        Description  = string.Empty,
        NodeType     = nodeType,
        OrderIndex   = orderIndex,
        Level        = level,
        Color        = color
    };
}
