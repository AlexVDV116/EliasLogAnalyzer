using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using System.Diagnostics;
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
        for (var i = 0; i < hash.Length; i++)
        {
            builder.Append(hash[i].ToString("x2"));
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
        for (int i = 0; i < fileHash.Length; i++)
        {
            fileHashBuilder.Append(fileHash[i].ToString("x2"));
        }

        return fileHashBuilder.ToString();
    }
}