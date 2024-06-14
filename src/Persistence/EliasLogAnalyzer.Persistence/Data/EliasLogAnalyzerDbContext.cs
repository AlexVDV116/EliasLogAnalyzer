using EliasLogAnalyzer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EliasLogAnalyzer.Persistence.Data;

public class EliasLogAnalyzerDbContext : DbContext
{
    public EliasLogAnalyzerDbContext(DbContextOptions<EliasLogAnalyzerDbContext> options) : base(options)
    {
    }

    public DbSet<LogFile> LogFiles { get; init; }
    public DbSet<LogEntry> LogEntries { get; init; }
    public DbSet<BugReport> BugReports { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BugReport>()
            .HasMany(b => b.LogEntriesToInclude)
            .WithMany(l => l.BugReports)
            .UsingEntity<BugReportLogEntry>(
        j => j
            .HasOne(ble => ble.LogEntry)
            .WithMany(le => le.BugReportLogEntries)
            .HasForeignKey(ble => ble.LogEntryId),
        j => j
            .HasOne(ble => ble.BugReport)
            .WithMany(br => br.BugReportLogEntries)
            .HasForeignKey(ble => ble.BugReportId),
        j =>
        {
            j.ToTable("BugReportLogEntries");
            j.HasKey(ble => new { ble.BugReportId, ble.LogEntryId }); // Composite key
        });

        modelBuilder.Entity<LogEntry>()
            .Ignore(le => le.IsPinned)
            .Ignore(le => le.IsMarked)
            .Ignore(le => le.TimeDelta)
            .Ignore(le => le.Probability);

        modelBuilder.Entity<LogFile>()
            .Ignore(lf => lf.IsSelected);

        modelBuilder.Entity<LogEntry>()
            .OwnsOne(le => le.LogTimeStamp, a =>
            {
                a.Property(p => p.DateTime).HasColumnName("DateTime");
                a.Property(p => p.Ticks).HasColumnName("Ticks");
                a.Property(p => p.DateTimeSortValue).HasColumnName("DateTimeSortValue");
            });

        // Configure unique indexes for Hash properties
        modelBuilder.Entity<LogFile>()
            .HasIndex(lf => lf.Hash)
            .IsUnique();

        modelBuilder.Entity<LogEntry>()
            .HasIndex(le => le.Hash)
            .IsUnique();
    }
}
