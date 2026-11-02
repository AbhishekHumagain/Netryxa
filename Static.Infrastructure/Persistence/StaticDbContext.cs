using System.Reflection;
using Static.Application.Common.Interfaces;
using Static.Domain.Entities;
using Static.Domain.Entities.Playground;
using Static.Domain.Entities.Srs;
using Static.Domain.Entities.Wbs;
using Static.EntityFrameworkCore.Extensions;
using Static.Infrastructure.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Static.Infrastructure.Persistence.Interceptors;

namespace Static.Infrastructure.Persistence;

public class StaticDbContext(
    DbContextOptions<StaticDbContext> options,
    IMediator? mediator = null,
    AuditingSaveChangesInterceptor? auditingSaveChangesInterceptor = null)
    : DbContext(options), IStaticDbContext

{
    public DbSet<DefaultSetting> DefaultSetting => Set<DefaultSetting>();
    public DbSet<UserNotificationSocket> UserNotificationSocket => Set<UserNotificationSocket>();
    public DbSet<UserNotification> UserNotification => Set<UserNotification>();
    public DbSet<User> User => Set<User>();
    public DbSet<UserLoginAttempt> UserLoginAttempt => Set<UserLoginAttempt>();
    public DbSet<Lesson> Lesson => Set<Lesson>();
    public DbSet<CodeExample> CodeExample => Set<CodeExample>();
    public DbSet<LessonContentBlock> LessonContentBlock => Set<LessonContentBlock>();
    public DbSet<CodingChallenge> CodingChallenge => Set<CodingChallenge>();
    public DbSet<QuizQuestion> QuizQuestion => Set<QuizQuestion>();
    public DbSet<LessonProgress> LessonProgress => Set<LessonProgress>();
    public DbSet<UmlDiagram> UmlDiagram => Set<UmlDiagram>();

    // ── SRS Editor ────────────────────────────────────────────────────────────
    public DbSet<SrsTemplate> SrsTemplate => Set<SrsTemplate>();
    public DbSet<SrsTemplateSectionDefinition> SrsTemplateSectionDefinition => Set<SrsTemplateSectionDefinition>();
    public DbSet<SrsDocument> SrsDocument => Set<SrsDocument>();
    public DbSet<SrsSection> SrsSection => Set<SrsSection>();

    // ── WBS Planner ───────────────────────────────────────────────────────────
    public DbSet<WbsTemplate> WbsTemplate => Set<WbsTemplate>();
    public DbSet<WbsTemplateNode> WbsTemplateNode => Set<WbsTemplateNode>();
    public DbSet<WbsDocument> WbsDocument => Set<WbsDocument>();
    public DbSet<WbsNode> WbsNode => Set<WbsNode>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        builder.Entity<User>()
               .HasIndex(d => d.EmailAddress)
               .IsUnique();
        builder.Entity<User>()
               .HasIndex(d => d.NickName)
               .IsUnique();

        // Playground — JSON payloads are stored as jsonb
        builder.Entity<CodeExample>()
               .Property(e => e.WalkthroughSteps)
               .HasColumnType("jsonb");

        builder.Entity<LessonContentBlock>(b =>
        {
            b.Property(x => x.Data).HasColumnType("jsonb");
            b.Property(x => x.Title).HasMaxLength(200);
            b.HasIndex(x => new { x.LessonId, x.Step, x.OrderIndex });
            b.HasOne(x => x.Lesson)
             .WithMany(l => l.ContentBlocks)
             .HasForeignKey(x => x.LessonId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CodingChallenge>(b =>
        {
            b.Property(x => x.Hints).HasColumnType("jsonb");
            b.Property(x => x.TestCases).HasColumnType("jsonb");
            b.Property(x => x.Title).HasMaxLength(200);
            b.Property(x => x.Difficulty).HasMaxLength(50);
            b.Property(x => x.Language).HasMaxLength(50);
            b.HasOne(x => x.Lesson)
             .WithMany(l => l.Challenges)
             .HasForeignKey(x => x.LessonId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<QuizQuestion>(b =>
        {
            b.Property(x => x.Options).HasColumnType("jsonb");
            b.HasOne(x => x.Lesson)
             .WithMany(l => l.QuizQuestions)
             .HasForeignKey(x => x.LessonId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<LessonProgress>(b =>
        {
            b.Property(x => x.VisitedSteps).HasColumnType("jsonb");
            b.Property(x => x.QuizAnswers).HasColumnType("jsonb");
            b.Property(x => x.ChallengesPassed).HasColumnType("jsonb");
            b.HasIndex(x => new { x.UserId, x.LessonId })
             .IsUnique()
             .HasFilter("\"IsDeleted\" = false");
            b.HasOne(x => x.Lesson)
             .WithMany()
             .HasForeignKey(x => x.LessonId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Lesson>()
               .Property(l => l.Difficulty)
               .HasMaxLength(50);

        // SRS relationships
        builder.Entity<SrsTemplate>()
               .HasMany(t => t.SectionDefinitions)
               .WithOne(s => s.Template)
               .HasForeignKey(s => s.TemplateId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<SrsDocument>()
               .HasMany(d => d.Sections)
               .WithOne(s => s.Document)
               .HasForeignKey(s => s.DocumentId)
               .OnDelete(DeleteBehavior.Cascade);

        // WBS relationships
        builder.Entity<WbsTemplate>()
               .HasMany(t => t.Nodes)
               .WithOne(n => n.Template)
               .HasForeignKey(n => n.TemplateId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<WbsTemplateNode>()
               .HasOne<WbsTemplateNode>()
               .WithMany()
               .HasForeignKey(n => n.ParentNodeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);

        builder.Entity<WbsDocument>()
               .HasMany(d => d.Nodes)
               .WithOne(n => n.Document)
               .HasForeignKey(n => n.DocumentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<WbsNode>()
               .HasOne<WbsNode>()
               .WithMany()
               .HasForeignKey(n => n.ParentNodeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);

        builder.ApplySoftDeleteQueryFilter();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (auditingSaveChangesInterceptor != null)
        {
            optionsBuilder.AddInterceptors(auditingSaveChangesInterceptor);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (mediator != null)
        {
            await mediator.DispatchDomainEvents(this);
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
