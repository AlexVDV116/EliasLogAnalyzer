using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EliasLogAnalyzer.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BugReports",
                columns: table => new
                {
                    BugReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeveloperName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkstationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Situation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expectation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Build = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Analysis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PossibleSolutions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhatToTest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Effort = table.Column<double>(type: "float", nullable: false),
                    Risk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Workaround = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugReports", x => x.BugReportId);
                });

            migrationBuilder.CreateTable(
                name: "LogFiles",
                columns: table => new
                {
                    LogFileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Computer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogFiles", x => x.LogFileId);
                });

            migrationBuilder.CreateTable(
                name: "LogEntries",
                columns: table => new
                {
                    LogEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeSortValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ticks = table.Column<long>(type: "bigint", nullable: false),
                    LogType = table.Column<int>(type: "int", nullable: false),
                    ThreadNameOrNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Computer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogFileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntries", x => x.LogEntryId);
                    table.ForeignKey(
                        name: "FK_LogEntries_LogFiles_LogFileId",
                        column: x => x.LogFileId,
                        principalTable: "LogFiles",
                        principalColumn: "LogFileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BugReportLogEntries",
                columns: table => new
                {
                    BugReportId = table.Column<int>(type: "int", nullable: false),
                    LogEntryId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugReportLogEntries", x => new { x.BugReportId, x.LogEntryId });
                    table.ForeignKey(
                        name: "FK_BugReportLogEntries_BugReports_BugReportId",
                        column: x => x.BugReportId,
                        principalTable: "BugReports",
                        principalColumn: "BugReportId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugReportLogEntries_LogEntries_LogEntryId",
                        column: x => x.LogEntryId,
                        principalTable: "LogEntries",
                        principalColumn: "LogEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BugReportLogEntries_LogEntryId",
                table: "BugReportLogEntries",
                column: "LogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Hash",
                table: "LogEntries",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_LogFileId",
                table: "LogEntries",
                column: "LogFileId");

            migrationBuilder.CreateIndex(
                name: "IX_LogFiles_Hash",
                table: "LogFiles",
                column: "Hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugReportLogEntries");

            migrationBuilder.DropTable(
                name: "BugReports");

            migrationBuilder.DropTable(
                name: "LogEntries");

            migrationBuilder.DropTable(
                name: "LogFiles");
        }
    }
}
