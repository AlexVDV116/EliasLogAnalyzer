using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace EliasLogAnalyzer.MAUI.Services;

public class HashService : IHashService
{
    public string GenerateLogEntryHash(LogEntry logEntry)
    {
        var rawData = $"{logEntry.LogTimeStamp.DateTime:o}_{logEntry.LogType}_{logEntry.ThreadNameOrNumber}_{logEntry.SourceLocation}_{logEntry.Source}_{logEntry.Category}_{logEntry.EventId}_{logEntry.User}_{logEntry.Computer}_{logEntry.Description}_{logEntry.Data}";
        var bytes = Encoding.UTF8.GetBytes(rawData);
        var hash = SHA256.HashData(bytes);

        var builder = new StringBuilder();
        foreach (var t in hash)
        {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString();
    }

    public string GenerateLogFileHash(LogFile logFile)
    {
        var entryHashes = new StringBuilder();
        foreach (var entry in logFile.LogEntries)
        {
            var logEntryHash = GenerateLogEntryHash(entry);
            entryHashes.Append(logEntryHash);
        }

        var fileData = $"{logFile.FileName}_{logFile.Computer}_{entryHashes}";
        var fileBytes = Encoding.UTF8.GetBytes(fileData);
        var fileHash = SHA256.HashData(fileBytes);

        var fileHashBuilder = new StringBuilder();
        foreach (var t in fileHash)
        {
            fileHashBuilder.Append(t.ToString("x2"));
        }

        return fileHashBuilder.ToString();
    }
}