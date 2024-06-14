﻿// <auto-generated />
using System;
using EliasLogAnalyzer.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EliasLogAnalyzer.Persistence.Migrations
{
    [DbContext(typeof(EliasLogAnalyzerDbContext))]
    partial class EliasLogAnalyzerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EliasLogAnalyzer.Domain.Entities.BugReport", b =>
                {
                    b.Property<int>("BugReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BugReportId"));

                    b.Property<string>("Analysis")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PossibleSolutions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Recommendation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ReportDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Risk")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BugReportId");

                    b.ToTable("BugReports");
                });

            modelBuilder.Entity("EliasLogAnalyzer.Domain.Entities.BugReportLogEntry", b =>
                {
                    b.Property<int>("BugReportId")
                        .HasColumnType("int");

                    b.Property<int>("LogEntryId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("BugReportId", "LogEntryId");

                    b.HasIndex("LogEntryId");

                    b.ToTable("BugReportLogEntries", (string)null);
                });

            modelBuilder.Entity("EliasLogAnalyzer.Domain.Entities.LogEntry", b =>
                {
                    b.Property<int>("LogEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LogEntryId"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Computer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("LogFileId")
                        .HasColumnType("int");

                    b.Property<int>("LogType")
                        .HasColumnType("int");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SourceLocation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ThreadNameOrNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LogEntryId");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.HasIndex("LogFileId");

                    b.ToTable("LogEntries");
                });

            modelBuilder.Entity("EliasLogAnalyzer.Domain.Entities.LogFile", b =>
                {
                    b.Property<int>("LogFileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LogFileId"));

                    b.Property<string>("Computer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LogFileId");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.ToTable("LogFiles");
                });

            modelBuilder.Entity("EliasLogAnalyzer.Domain.Entities.BugReportLogEntry", b =>
                {
                    b.HasOne("EliasLogAnalyzer.Domain.Entities.BugReport", "BugReport")
                        .WithMany("BugReportLogEntries")
                        .HasForeignKey("BugReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EliasLogAnalyzer.Domain.Entities.LogEntry", "LogEntry")
                        .WithMany("BugReportLogEntries")
                        .HasForeignKey("LogEntryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BugReport");

                    b.Navigation("LogEntry");
                });

            modelBuilder.Entity("EliasLogAnalyzer.Domain.Entities.LogEntry", b =>
                {
                    b.HasOne("EliasLogAnalyzer.Domain.Entities.LogFile", "LogFile")
                        .WithMany()
                        .HasForeignKey("LogFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("EliasLogAnalyzer.Domain.Entities.LogTimestamp", "LogTimeStamp", b1 =>
                        {
                            b1.Property<int>("LogEntryId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("DateTime")
                                .HasColumnType("datetime2")
                                .HasColumnName("DateTime");

                            b1.Property<string>("DateTimeSortValue")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("DateTimeSortValue");

                            b1.Property<long>("Ticks")
                                .HasColumnType("bigint")
                                .HasColumnName("Ticks");

                            b1.HasKey("LogEntryId");

                            b1.ToTable("LogEntries");

                            b1.WithOwner()
                                .HasForeignKey("LogEntryId");
                        });

                    b.Navigation("LogFile");

                    b.Navigation("LogTimeStamp")
                        .IsRequired();
                });

            modelBuilder.Entity("EliasLogAnalyzer.Domain.Entities.BugReport", b =>
                {
                    b.Navigation("BugReportLogEntries");
                });

            modelBuilder.Entity("EliasLogAnalyzer.Domain.Entities.LogEntry", b =>
                {
                    b.Navigation("BugReportLogEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
