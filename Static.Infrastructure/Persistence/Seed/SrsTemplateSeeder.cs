using Microsoft.EntityFrameworkCore;
using Static.Domain.Entities.Srs;
using Static.Domain.Enums;

namespace Static.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds the built-in SRS templates for each supported development methodology.
/// Section content is stored as rich-text HTML (compatible with Quill / TipTap editors).
/// Templates are idempotent — re-running does nothing if templates already exist.
/// </summary>
public sealed class SrsTemplateSeeder(StaticDbContext db) : ISeeder
{
    public async Task SeedAsync()
    {
        if (await db.SrsTemplate.AnyAsync())
            return;

        var templates = BuildTemplates();
        await db.SrsTemplate.AddRangeAsync(templates);
        await db.SaveChangesAsync();
    }

    private static List<SrsTemplate> BuildTemplates() =>
    [
        BuildWaterfallTemplate(),
        BuildAgileTemplate(),
        BuildScrumTemplate(),
        BuildKanbanTemplate(),
        BuildRupTemplate(),
        BuildCustomTemplate()
    ];

    // ── 1. Waterfall (IEEE 830-2008) ──────────────────────────────────────────

    private static SrsTemplate BuildWaterfallTemplate() => new()
    {
        Id          = Guid.NewGuid(),
        Name        = "Waterfall — IEEE 830",
        Description = "Classic IEEE 830-2008 SRS structure for sequential, plan-driven projects. Covers all requirements before development begins.",
        Methodology = SrsMethodology.Waterfall,
        IsActive    = true,
        SectionDefinitions =
        [
            Sec(1, "Introduction", "introduction", true,
                "<h2>1. Introduction</h2>" +
                "<h3>1.1 Purpose</h3>" +
                "<p>Describe the purpose of this SRS document and the intended audience.</p>" +
                "<h3>1.2 Scope</h3>" +
                "<p>Identify the software product(s) to be produced by name; explain what the product will (and will not) do.</p>" +
                "<h3>1.3 Definitions, Acronyms, and Abbreviations</h3>" +
                "<table><tbody>" +
                "<tr><th>Term</th><th>Definition</th></tr>" +
                "<tr><td>SRS</td><td>Software Requirements Specification</td></tr>" +
                "<tr><td>TBD</td><td>To Be Determined</td></tr>" +
                "</tbody></table>" +
                "<h3>1.4 References</h3>" +
                "<p>List any documents or standards referenced in this SRS.</p>" +
                "<h3>1.5 Overview</h3>" +
                "<p>Describe what the rest of the SRS contains and how it is organised.</p>",
                "Define purpose, scope, key terms, references, and document structure."),

            Sec(2, "Overall Description", "overall-description", true,
                "<h2>2. Overall Description</h2>" +
                "<h3>2.1 Product Perspective</h3>" +
                "<p>Describe the context and origin of the product (new, next generation, part of a system, replacement, etc.).</p>" +
                "<h3>2.2 Product Functions</h3>" +
                "<p>Provide a high-level summary of the major functions the product will perform.</p>" +
                "<ul><li>Function 1</li><li>Function 2</li><li>Function 3</li></ul>" +
                "<h3>2.3 User Characteristics</h3>" +
                "<p>Describe the general characteristics and expertise of the expected users.</p>" +
                "<h3>2.4 Constraints</h3>" +
                "<p>List any constraints that limit the design options (hardware, software, standards, policies).</p>" +
                "<h3>2.5 Assumptions and Dependencies</h3>" +
                "<p>List any assumed facts or dependencies that may affect the requirements.</p>",
                "Product context, high-level functions, users, constraints, and assumptions."),

            Sec(3, "Functional Requirements", "functional-requirements", true,
                "<h2>3.1 Functional Requirements</h2>" +
                "<p>Use the template below for each functional requirement.</p>" +
                "<h3>FR-001: [Requirement Title]</h3>" +
                "<ul>" +
                "<li><strong>Description:</strong> What the system shall do.</li>" +
                "<li><strong>Inputs:</strong> Data or events that trigger this behaviour.</li>" +
                "<li><strong>Processing:</strong> The transformation or logic applied.</li>" +
                "<li><strong>Outputs:</strong> Data or state produced.</li>" +
                "<li><strong>Priority:</strong> High / Medium / Low</li>" +
                "<li><strong>Source:</strong> [Stakeholder or document]</li>" +
                "</ul>" +
                "<h3>FR-002: [Requirement Title]</h3>" +
                "<p><em>Add more requirements as needed.</em></p>",
                "Detail every function the system must perform, with inputs, outputs, and priority."),

            Sec(4, "Non-Functional Requirements", "non-functional-requirements", true,
                "<h2>3.2 Non-Functional Requirements</h2>" +
                "<h3>Performance</h3>" +
                "<ul>" +
                "<li>The system shall respond to user requests within <strong>2 seconds</strong> under normal load.</li>" +
                "<li>The system shall support up to <strong>1,000 concurrent users</strong>.</li>" +
                "</ul>" +
                "<h3>Security</h3>" +
                "<ul>" +
                "<li>All data in transit shall be encrypted using TLS 1.3 or higher.</li>" +
                "<li>User passwords shall be hashed using bcrypt with a minimum cost factor of 12.</li>" +
                "</ul>" +
                "<h3>Reliability &amp; Availability</h3>" +
                "<ul>" +
                "<li>The system shall achieve <strong>99.9% uptime</strong> per calendar month.</li>" +
                "<li>Planned maintenance windows shall not exceed 4 hours per month.</li>" +
                "</ul>" +
                "<h3>Scalability</h3>" +
                "<ul><li>The architecture shall support horizontal scaling without code changes.</li></ul>" +
                "<h3>Maintainability</h3>" +
                "<ul><li>Code coverage shall be at least <strong>80%</strong> as measured by automated tests.</li></ul>" +
                "<h3>Usability</h3>" +
                "<ul><li>First-time users shall be able to complete core tasks without training.</li></ul>",
                "Specify performance, security, reliability, scalability, and usability constraints."),

            Sec(5, "External Interface Requirements", "external-interfaces", false,
                "<h2>3.3 External Interface Requirements</h2>" +
                "<h3>User Interfaces</h3>" +
                "<p>Describe the logical characteristics of each UI between the product and its users.</p>" +
                "<h3>Hardware Interfaces</h3>" +
                "<p>List any hardware interfaces the system must interact with.</p>" +
                "<h3>Software Interfaces</h3>" +
                "<p>Describe the connections with other software components (APIs, databases, third-party libraries).</p>" +
                "<table><tbody>" +
                "<tr><th>Interface</th><th>Type</th><th>Protocol</th><th>Description</th></tr>" +
                "<tr><td>Payment Gateway</td><td>REST API</td><td>HTTPS</td><td>Processes card payments</td></tr>" +
                "</tbody></table>" +
                "<h3>Communication Interfaces</h3>" +
                "<p>Describe network communications requirements (protocols, security, data rates).</p>",
                "Describe how the system interfaces with users, hardware, other software, and networks."),

            Sec(6, "Design Constraints", "design-constraints", false,
                "<h2>3.4 Design Constraints</h2>" +
                "<p>List any standards compliance, hardware limitations, or other factors that constrain design:</p>" +
                "<ul>" +
                "<li><strong>Programming Language:</strong> [e.g. C# / .NET 10]</li>" +
                "<li><strong>Database:</strong> [e.g. PostgreSQL 17]</li>" +
                "<li><strong>Standards:</strong> [e.g. ISO 27001, GDPR]</li>" +
                "<li><strong>Platform:</strong> [e.g. Linux containers on AWS]</li>" +
                "</ul>",
                "Standards compliance, hardware limits, and technology choices that constrain design."),

            Sec(7, "Appendices", "appendices", false,
                "<h2>Appendices</h2>" +
                "<h3>Appendix A — Data Dictionary</h3>" +
                "<p>Define all data elements referenced in this document.</p>" +
                "<h3>Appendix B — Analysis Models</h3>" +
                "<p>Include any UML diagrams, ER diagrams, or state machines that support the requirements.</p>" +
                "<h3>Appendix C — To Be Determined (TBD) List</h3>" +
                "<p>Track unresolved items here:</p>" +
                "<table><tbody>" +
                "<tr><th>ID</th><th>Description</th><th>Owner</th><th>Due Date</th></tr>" +
                "<tr><td></td><td></td><td></td><td></td></tr>" +
                "</tbody></table>",
                "Data dictionary, supporting models, and unresolved items.")
        ]
    };

    // ── 2. Agile ──────────────────────────────────────────────────────────────

    private static SrsTemplate BuildAgileTemplate() => new()
    {
        Id          = Guid.NewGuid(),
        Name        = "Agile SRS",
        Description = "Lightweight SRS for Agile teams. Captures product vision, user stories, and acceptance criteria iteratively.",
        Methodology = SrsMethodology.Agile,
        IsActive    = true,
        SectionDefinitions =
        [
            Sec(1, "Product Vision", "product-vision", true,
                "<h2>Product Vision</h2>" +
                "<p><strong>Elevator Pitch:</strong><br>" +
                "For <strong>[target users]</strong> who <strong>[need]</strong>, the <strong>[product name]</strong> is a <strong>[category]</strong> that <strong>[benefit]</strong>. Unlike <strong>[competitor/alternative]</strong>, our product <strong>[differentiator]</strong>.</p>" +
                "<h3>Goals</h3>" +
                "<ul><li>Goal 1</li><li>Goal 2</li></ul>" +
                "<h3>Success Metrics</h3>" +
                "<table><tbody>" +
                "<tr><th>Metric</th><th>Target</th><th>Measurement Method</th></tr>" +
                "<tr><td>User retention</td><td>&gt; 60% at 30 days</td><td>Analytics</td></tr>" +
                "</tbody></table>",
                "One-paragraph vision, goals, and measurable success metrics for the product."),

            Sec(2, "Stakeholders", "stakeholders", true,
                "<h2>Stakeholders</h2>" +
                "<table><tbody>" +
                "<tr><th>Role</th><th>Name / Team</th><th>Interest</th><th>Involvement</th></tr>" +
                "<tr><td>Product Owner</td><td></td><td>Defines priorities</td><td>High</td></tr>" +
                "<tr><td>End User</td><td>Students</td><td>Uses the product</td><td>High</td></tr>" +
                "<tr><td>Developer</td><td>Engineering Team</td><td>Builds the product</td><td>High</td></tr>" +
                "<tr><td>Sponsor</td><td>Management</td><td>Funds the project</td><td>Low</td></tr>" +
                "</tbody></table>",
                "List all parties with an interest in the product and their level of involvement."),

            Sec(3, "User Personas", "user-personas", false,
                "<h2>User Personas</h2>" +
                "<h3>Persona 1 — [Name]</h3>" +
                "<ul>" +
                "<li><strong>Role:</strong> [e.g. Student]</li>" +
                "<li><strong>Goal:</strong> [What they want to achieve]</li>" +
                "<li><strong>Pain Point:</strong> [What frustrates them today]</li>" +
                "<li><strong>Tech Savviness:</strong> Low / Medium / High</li>" +
                "</ul>" +
                "<h3>Persona 2 — [Name]</h3>" +
                "<p><em>Add more personas as needed.</em></p>",
                "Describe representative users — their goals, frustrations, and behaviours."),

            Sec(4, "User Stories & Epics", "user-stories", true,
                "<h2>User Stories &amp; Epics</h2>" +
                "<h3>Epic 1: [Epic Title]</h3>" +
                "<p><em>As a [type of user], I want [goal] so that [benefit].</em></p>" +
                "<table><tbody>" +
                "<tr><th>Story ID</th><th>User Story</th><th>Priority</th><th>Estimate</th></tr>" +
                "<tr><td>US-001</td><td>As a student, I want to log in with Google so that I don't manage a separate password.</td><td>High</td><td>3 pts</td></tr>" +
                "<tr><td>US-002</td><td>As a teacher, I want to create assignments so that students have structured work.</td><td>High</td><td>5 pts</td></tr>" +
                "</tbody></table>" +
                "<h3>Epic 2: [Epic Title]</h3>" +
                "<p><em>Add stories here.</em></p>",
                "Structured user stories grouped by epic with priority and story-point estimates."),

            Sec(5, "Acceptance Criteria", "acceptance-criteria", true,
                "<h2>Acceptance Criteria</h2>" +
                "<p>Use Gherkin (Given / When / Then) or bullet checklists for each user story.</p>" +
                "<h3>US-001 — Google Login</h3>" +
                "<ul>" +
                "<li><strong>Given</strong> a new user visits the login page</li>" +
                "<li><strong>When</strong> they click \"Sign in with Google\" and authorise access</li>" +
                "<li><strong>Then</strong> they are redirected to the dashboard and a user account is created</li>" +
                "</ul>" +
                "<h3>US-002 — Create Assignment</h3>" +
                "<ul>" +
                "<li>Teacher can set a title, description, and due date</li>" +
                "<li>Assignment appears in the student's task list immediately</li>" +
                "<li>System sends an email notification to enrolled students</li>" +
                "</ul>",
                "Define the conditions that must be satisfied for each user story to be accepted."),

            Sec(6, "Non-Functional Requirements", "non-functional-requirements", true,
                "<h2>Non-Functional Requirements</h2>" +
                "<table><tbody>" +
                "<tr><th>Category</th><th>Requirement</th><th>Target</th></tr>" +
                "<tr><td>Performance</td><td>Page load time</td><td>&lt; 2 s (p95)</td></tr>" +
                "<tr><td>Security</td><td>Authentication</td><td>OAuth 2.0 + JWT</td></tr>" +
                "<tr><td>Availability</td><td>Uptime</td><td>99.9%</td></tr>" +
                "<tr><td>Accessibility</td><td>WCAG compliance</td><td>Level AA</td></tr>" +
                "<tr><td>Scalability</td><td>Concurrent users</td><td>500+</td></tr>" +
                "</tbody></table>",
                "Quality attributes that cut across features: performance, security, and availability."),

            Sec(7, "Technical Constraints & Assumptions", "constraints-assumptions", false,
                "<h2>Technical Constraints &amp; Assumptions</h2>" +
                "<h3>Constraints</h3>" +
                "<ul>" +
                "<li>Must deploy to [cloud provider].</li>" +
                "<li>Must use existing [identity provider].</li>" +
                "<li>Budget: $X/month for infrastructure.</li>" +
                "</ul>" +
                "<h3>Assumptions</h3>" +
                "<ul>" +
                "<li>Users have a modern browser (Chrome 120+, Firefox 120+, Safari 17+).</li>" +
                "<li>Internet connectivity is always available.</li>" +
                "</ul>" +
                "<h3>Out of Scope</h3>" +
                "<ul>" +
                "<li>Feature A (deferred to Phase 2)</li>" +
                "<li>Feature B (not required by sponsor)</li>" +
                "</ul>",
                "Technical limits, assumptions about the environment, and features explicitly excluded.")
        ]
    };

    // ── 3. Scrum ──────────────────────────────────────────────────────────────

    private static SrsTemplate BuildScrumTemplate() => new()
    {
        Id          = Guid.NewGuid(),
        Name        = "Scrum SRS",
        Description = "Scrum-aligned SRS focusing on product backlog, sprint planning, and definition of done.",
        Methodology = SrsMethodology.Scrum,
        IsActive    = true,
        SectionDefinitions =
        [
            Sec(1, "Product Vision & Goals", "product-vision", true,
                "<h2>Product Vision</h2>" +
                "<p><strong>Vision Statement:</strong> In one sentence, describe the end-state the product enables.</p>" +
                "<h3>Sprint 0 Goals</h3>" +
                "<ul>" +
                "<li>Environment set up</li>" +
                "<li>CI/CD pipeline running</li>" +
                "<li>Core domain model agreed</li>" +
                "</ul>",
                "One-sentence vision and the Sprint 0 readiness checklist."),

            Sec(2, "Scrum Team", "scrum-team", true,
                "<h2>Scrum Team</h2>" +
                "<table><tbody>" +
                "<tr><th>Role</th><th>Name</th></tr>" +
                "<tr><td>Product Owner</td><td></td></tr>" +
                "<tr><td>Scrum Master</td><td></td></tr>" +
                "<tr><td>Developer</td><td></td></tr>" +
                "<tr><td>Developer</td><td></td></tr>" +
                "</tbody></table>" +
                "<p><strong>Velocity Baseline:</strong> [N] story points / sprint <em>(update after Sprint 2)</em><br>" +
                "<strong>Sprint Length:</strong> 2 weeks</p>",
                "List the Scrum team roles and their baseline velocity."),

            Sec(3, "Product Backlog", "product-backlog", true,
                "<h2>Product Backlog</h2>" +
                "<p>Ordered by business value. Top items are refined and ready for sprint planning.</p>" +
                "<table><tbody>" +
                "<tr><th>ID</th><th>Title</th><th>Type</th><th>Priority</th><th>Points</th><th>Status</th></tr>" +
                "<tr><td>PBI-001</td><td>User can register with email</td><td>Story</td><td>Must</td><td>3</td><td>Ready</td></tr>" +
                "<tr><td>PBI-002</td><td>Admin can view all users</td><td>Story</td><td>Should</td><td>2</td><td>Ready</td></tr>" +
                "<tr><td>PBI-003</td><td>System sends welcome email</td><td>Story</td><td>Could</td><td>1</td><td>Refined</td></tr>" +
                "</tbody></table>" +
                "<p><em>Use MoSCoW: <strong>Must / Should / Could / Won't</strong></em></p>",
                "The ordered list of work to be done, with story points and MoSCoW priority."),

            Sec(4, "Epic Definitions", "epics", true,
                "<h2>Epics</h2>" +
                "<h3>EPIC-001: Authentication &amp; Authorisation</h3>" +
                "<p><strong>Goal:</strong> Allow users to safely create accounts and log in.<br>" +
                "<strong>Stories:</strong> PBI-001, PBI-002<br>" +
                "<strong>Acceptance:</strong> All defined stories are done and security scan passes.</p>" +
                "<h3>EPIC-002: [Title]</h3>" +
                "<p><em>Add epics as needed.</em></p>",
                "Describe large features (epics) that span multiple sprints and list their constituent stories."),

            Sec(5, "Definition of Ready", "definition-of-ready", true,
                "<h2>Definition of Ready (DoR)</h2>" +
                "<p>A backlog item is ready to enter a sprint when:</p>" +
                "<ul>" +
                "<li>Written as a user story (<em>\"As a … I want … so that …\"</em>)</li>" +
                "<li>Acceptance criteria are defined and agreed</li>" +
                "<li>Story points estimated by the team</li>" +
                "<li>Dependencies identified and resolved (or tracked)</li>" +
                "<li>Design mock-up available (for UI stories)</li>" +
                "</ul>",
                "Checklist a backlog item must satisfy before it can be pulled into a sprint."),

            Sec(6, "Definition of Done", "definition-of-done", true,
                "<h2>Definition of Done (DoD)</h2>" +
                "<p>A story is done when:</p>" +
                "<ul>" +
                "<li>All acceptance criteria pass</li>" +
                "<li>Unit tests written and passing (coverage ≥ 80%)</li>" +
                "<li>Code reviewed and approved by at least 1 peer</li>" +
                "<li>No new high/critical SonarQube issues</li>" +
                "<li>Deployed to staging environment</li>" +
                "<li>Product Owner has accepted the story</li>" +
                "</ul>",
                "Checklist every story must satisfy before it can be marked complete."),

            Sec(7, "Non-Functional Requirements", "non-functional-requirements", false,
                "<h2>Non-Functional Requirements</h2>" +
                "<p>These apply across all sprints and are validated at each sprint review.</p>" +
                "<table><tbody>" +
                "<tr><th>NFR</th><th>Requirement</th><th>Verification</th></tr>" +
                "<tr><td>Performance</td><td>API P95 latency &lt; 300 ms</td><td>Load test</td></tr>" +
                "<tr><td>Security</td><td>OWASP Top-10 mitigated</td><td>Penetration test</td></tr>" +
                "<tr><td>Accessibility</td><td>WCAG 2.1 AA</td><td>Axe automated scan</td></tr>" +
                "</tbody></table>",
                "Quality attributes tracked and verified across all sprints."),

            Sec(8, "Sprint Log", "sprint-log", false,
                "<h2>Sprint Log</h2>" +
                "<p>Update this section after each sprint review.</p>" +
                "<table><tbody>" +
                "<tr><th>Sprint</th><th>Goal</th><th>Velocity</th><th>Key Deliverables</th></tr>" +
                "<tr><td>1</td><td>Auth MVP</td><td></td><td></td></tr>" +
                "<tr><td>2</td><td></td><td></td><td></td></tr>" +
                "</tbody></table>",
                "Running record of sprint goals, achieved velocity, and shipped features.")
        ]
    };

    // ── 4. Kanban ─────────────────────────────────────────────────────────────

    private static SrsTemplate BuildKanbanTemplate() => new()
    {
        Id          = Guid.NewGuid(),
        Name        = "Kanban SRS",
        Description = "Continuous-flow SRS for Kanban teams. Focuses on workflow, WIP limits, and SLAs.",
        Methodology = SrsMethodology.Kanban,
        IsActive    = true,
        SectionDefinitions =
        [
            Sec(1, "Project Overview", "overview", true,
                "<h2>Project Overview</h2>" +
                "<p><strong>Product:</strong> [Name]<br>" +
                "<strong>Team:</strong> [Team name / members]<br>" +
                "<strong>Start Date:</strong> [Date]<br>" +
                "<strong>Service Type:</strong> Continuous delivery / Maintenance / New development</p>" +
                "<h3>Mission Statement</h3>" +
                "<p>One sentence describing the purpose of this product or service.</p>",
                "Brief description of the product, team, and delivery model."),

            Sec(2, "Workflow Definition", "workflow", true,
                "<h2>Workflow Definition</h2>" +
                "<table><tbody>" +
                "<tr><th>Column</th><th>Definition of Entry</th><th>Definition of Exit</th></tr>" +
                "<tr><td>Backlog</td><td>Item is written and understood</td><td>Prioritised and pulled</td></tr>" +
                "<tr><td>Analysis</td><td>Dev pulls item</td><td>Requirements clarified</td></tr>" +
                "<tr><td>Development</td><td>Coding starts</td><td>Code complete, tests passing</td></tr>" +
                "<tr><td>Review</td><td>PR opened</td><td>PR approved</td></tr>" +
                "<tr><td>Done</td><td>Deployed to production</td><td>—</td></tr>" +
                "</tbody></table>" +
                "<p><strong>Blocked Policy:</strong> Blocked items are highlighted with a 🔴 tag. The team swarms on blockers daily.</p>",
                "Define each stage of the workflow, entry/exit criteria, and blocked-item policy."),

            Sec(3, "Work Item Types", "work-item-types", true,
                "<h2>Work Item Types</h2>" +
                "<table><tbody>" +
                "<tr><th>Type</th><th>Description</th><th>Example</th></tr>" +
                "<tr><td><strong>Feature</strong></td><td>New user-visible functionality</td><td>\"Add dark mode\"</td></tr>" +
                "<tr><td><strong>Bug</strong></td><td>Defect fix</td><td>\"Login fails on Safari\"</td></tr>" +
                "<tr><td><strong>Tech Debt</strong></td><td>Refactoring or infrastructure improvement</td><td>\"Migrate to .NET 10\"</td></tr>" +
                "<tr><td><strong>Spike</strong></td><td>Time-boxed research</td><td>\"Evaluate Redis vs Memcached\"</td></tr>" +
                "</tbody></table>",
                "Define the categories of work items flowing through the system."),

            Sec(4, "WIP Limits", "wip-limits", true,
                "<h2>WIP Limits</h2>" +
                "<table><tbody>" +
                "<tr><th>Column</th><th>WIP Limit</th><th>Rationale</th></tr>" +
                "<tr><td>Analysis</td><td>3</td><td>Prevent overloading the team</td></tr>" +
                "<tr><td>Development</td><td>4</td><td>Two devs × 2 items each</td></tr>" +
                "<tr><td>Review</td><td>2</td><td>Keep PRs small and fast</td></tr>" +
                "</tbody></table>" +
                "<h3>Class of Service</h3>" +
                "<ul>" +
                "<li>🔴 <strong>Expedite</strong> (max 1 in flight): critical production defects</li>" +
                "<li>🟡 <strong>Fixed Date</strong>: items with hard external deadlines</li>" +
                "<li>🟢 <strong>Standard</strong>: normal flow</li>" +
                "</ul>",
                "Work-in-progress limits per column and class-of-service definitions."),

            Sec(5, "Service Level Agreements", "slas", true,
                "<h2>Service Level Agreements (SLAs)</h2>" +
                "<table><tbody>" +
                "<tr><th>Class of Service</th><th>Target Lead Time</th><th>Target Cycle Time</th></tr>" +
                "<tr><td>Expedite</td><td>1 day</td><td>&lt; 4 hours</td></tr>" +
                "<tr><td>Fixed Date</td><td>Per commitment</td><td>≤ 5 days</td></tr>" +
                "<tr><td>Standard</td><td>10 days</td><td>≤ 7 days</td></tr>" +
                "</tbody></table>" +
                "<p><strong>Lead Time</strong> = time from item creation to deployment.<br>" +
                "<strong>Cycle Time</strong> = time from work start to deployment.</p>",
                "Define lead time and cycle time targets for each class of service."),

            Sec(6, "Continuous Improvement", "continuous-improvement", false,
                "<h2>Continuous Improvement</h2>" +
                "<h3>Cadence</h3>" +
                "<ul>" +
                "<li><strong>Daily Standup:</strong> 15 min — walk the board right-to-left</li>" +
                "<li><strong>Weekly Flow Review:</strong> Review cumulative flow diagram, spot bottlenecks</li>" +
                "<li><strong>Monthly Retrospective:</strong> Process improvement actions</li>" +
                "</ul>" +
                "<h3>Key Metrics</h3>" +
                "<ul>" +
                "<li>Throughput (items/week)</li>" +
                "<li>Lead time distribution</li>" +
                "<li>Blocked time per item</li>" +
                "<li>Escaped defects</li>" +
                "</ul>",
                "Meeting cadence, flow metrics, and improvement process for the Kanban team.")
        ]
    };

    // ── 5. RUP ───────────────────────────────────────────────────────────────

    private static SrsTemplate BuildRupTemplate() => new()
    {
        Id          = Guid.NewGuid(),
        Name        = "RUP (Rational Unified Process) SRS",
        Description = "Use-case-driven SRS following the Rational Unified Process. Captures actors, use cases, and supplementary specifications.",
        Methodology = SrsMethodology.RUP,
        IsActive    = true,
        SectionDefinitions =
        [
            Sec(1, "Introduction", "introduction", true,
                "<h2>Introduction</h2>" +
                "<h3>1.1 Purpose</h3>" +
                "<p>State the purpose of this document and the system it describes.</p>" +
                "<h3>1.2 Scope</h3>" +
                "<p>Briefly describe the system under development and its context.</p>" +
                "<h3>1.3 Definitions and Acronyms</h3>" +
                "<table><tbody>" +
                "<tr><th>Term</th><th>Definition</th></tr>" +
                "<tr><td>UC</td><td>Use Case</td></tr>" +
                "<tr><td>Actor</td><td>A role interacting with the system</td></tr>" +
                "</tbody></table>" +
                "<h3>1.4 Overview</h3>" +
                "<p>Describe the structure of this document.</p>",
                "Document purpose, system scope, key terms, and document structure."),

            Sec(2, "Use Case Model Survey", "use-case-survey", true,
                "<h2>Use Case Model Survey</h2>" +
                "<p>List the major use cases and their brief descriptions.</p>" +
                "<table><tbody>" +
                "<tr><th>UC ID</th><th>Name</th><th>Primary Actor</th><th>Priority</th></tr>" +
                "<tr><td>UC-001</td><td>Login</td><td>Student</td><td>High</td></tr>" +
                "<tr><td>UC-002</td><td>Submit Assignment</td><td>Student</td><td>High</td></tr>" +
                "<tr><td>UC-003</td><td>Grade Assignment</td><td>Teacher</td><td>Medium</td></tr>" +
                "</tbody></table>" +
                "<p><em>Attach a Use Case Diagram image or link here.</em></p>",
                "High-level catalogue of all use cases with primary actors and priorities."),

            Sec(3, "Actors and Goals", "actors-goals", true,
                "<h2>Actors and Goals</h2>" +
                "<h3>Actor: Student</h3>" +
                "<ul><li><strong>Goal:</strong> Complete and submit assignments; track academic progress.</li></ul>" +
                "<h3>Actor: Teacher</h3>" +
                "<ul><li><strong>Goal:</strong> Create lessons, grade work, monitor student performance.</li></ul>" +
                "<h3>Actor: System Administrator</h3>" +
                "<ul><li><strong>Goal:</strong> Manage user accounts and system configuration.</li></ul>" +
                "<h3>Actor: External System — Email Service</h3>" +
                "<ul><li><strong>Goal:</strong> Deliver notifications triggered by system events.</li></ul>",
                "Describe each actor (human or external system) and their primary goals."),

            Sec(4, "Use Case Descriptions", "use-case-descriptions", true,
                "<h2>Use Case Descriptions</h2>" +
                "<h3>UC-001: Login</h3>" +
                "<table><tbody>" +
                "<tr><th>Field</th><th>Detail</th></tr>" +
                "<tr><td><strong>ID</strong></td><td>UC-001</td></tr>" +
                "<tr><td><strong>Name</strong></td><td>Login</td></tr>" +
                "<tr><td><strong>Primary Actor</strong></td><td>Student, Teacher</td></tr>" +
                "<tr><td><strong>Preconditions</strong></td><td>User has a registered account</td></tr>" +
                "<tr><td><strong>Postconditions</strong></td><td>User is authenticated; session created</td></tr>" +
                "<tr><td><strong>Basic Flow</strong></td><td>1. User navigates to login page.<br>2. User enters credentials.<br>3. System validates and creates session.<br>4. User is redirected to dashboard.</td></tr>" +
                "<tr><td><strong>Alternative Flow</strong></td><td>3a. Invalid credentials → system shows error; allow 3 retries.</td></tr>" +
                "<tr><td><strong>Exception Flow</strong></td><td>3b. Account locked → show support contact.</td></tr>" +
                "<tr><td><strong>Priority</strong></td><td>High</td></tr>" +
                "</tbody></table>" +
                "<p><em>Duplicate this table for each use case.</em></p>",
                "Full structured description (preconditions, basic/alternative/exception flows) for each use case."),

            Sec(5, "Supplementary Specifications", "supplementary-specs", true,
                "<h2>Supplementary Specifications (FURPS+)</h2>" +
                "<h3>Functionality</h3>" +
                "<p>Capabilities not captured in use cases (e.g. audit logging, data retention).</p>" +
                "<h3>Usability</h3>" +
                "<ul><li>Interface shall follow WCAG 2.1 AA accessibility guidelines.</li></ul>" +
                "<h3>Reliability</h3>" +
                "<ul><li>System availability: 99.9% per month.</li></ul>" +
                "<h3>Performance</h3>" +
                "<ul><li>All use case responses ≤ 2 seconds under normal load.</li></ul>" +
                "<h3>Supportability</h3>" +
                "<ul><li>Logs shall be structured JSON compatible with ELK stack.</li></ul>" +
                "<h3>Constraints (+)</h3>" +
                "<p>Design, implementation, interface, and physical constraints.</p>",
                "FURPS+ quality attributes not expressed as use case flows."),

            Sec(6, "Glossary", "glossary", false,
                "<h2>Glossary</h2>" +
                "<table><tbody>" +
                "<tr><th>Term</th><th>Definition</th></tr>" +
                "<tr><td>Assignment</td><td>A task set by a teacher that students must complete by a due date.</td></tr>" +
                "<tr><td>Session</td><td>An authenticated user's active connection to the system.</td></tr>" +
                "</tbody></table>",
                "Define domain-specific terms used throughout this document.")
        ]
    };

    // ── 6. Custom (blank) ─────────────────────────────────────────────────────

    private static SrsTemplate BuildCustomTemplate() => new()
    {
        Id          = Guid.NewGuid(),
        Name        = "Custom (Blank)",
        Description = "Start with a single blank section and build your own structure from scratch.",
        Methodology = SrsMethodology.Custom,
        IsActive    = true,
        SectionDefinitions =
        [
            Sec(1, "Overview", "overview", false,
                "<h2>Overview</h2>" +
                "<p>Write a brief description of the system or feature this SRS covers.</p>" +
                "<p><em>Tip: Use the \"+ Add Section\" button to build any structure you need — there are no required sections in this template.</em></p>",
                "A single blank starting section — add, rename, and reorder sections freely.")
        ]
    };

    // ── Helper ────────────────────────────────────────────────────────────────

    private static SrsTemplateSectionDefinition Sec(
        int    order,
        string title,
        string key,
        bool   isRequired,
        string htmlContent,
        string guidanceNotes) => new()
    {
        Id            = Guid.NewGuid(),
        Title         = title,
        SectionKey    = key,
        SampleContent = htmlContent,
        GuidanceNotes = guidanceNotes,
        OrderIndex    = order,
        IsRequired    = isRequired
    };
}
