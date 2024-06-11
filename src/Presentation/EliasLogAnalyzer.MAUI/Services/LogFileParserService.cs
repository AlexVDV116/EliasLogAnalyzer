using System.Globalization;
using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;

namespace EliasLogAnalyzer.MAUI.Services;

/// <summary>
/// Service that provides functionality to parse log files into structured log entries.
/// </summary>
public class LogFileParserService(ILogger<LogFileParserService> logger, IHashService hashService) : ILogFileParserService
{
    private const string DateFormat = "dd-MM-yyyy";
    private const string TimeFormat = "HH:mm:ss";

    /// <summary>
    /// Parses a log file and creates a structured LogFile object.
    /// </summary>
    /// <param name="fileResult">File to parse.</param>
    /// <returns>A LogFile object containing parsed log entries.</returns>
    public async Task<LogFile> ParseLogFileAsync(FileResult fileResult)
    {
        var entries = await ParseLogEntriesAsync(fileResult);
        var logFile = new LogFile
        {
            FileName = fileResult.FileName,
            FullPath = fileResult.FullPath,
            Computer = entries.FirstOrDefault()?.Computer ?? "Unknown",
            LogEntries = entries
        };

        logFile.Hash = hashService.GenerateLogFileHash(logFile);

        foreach (var entry in entries)
        {
            entry.LogFile = logFile; // Associate the LogFile with each LogEntry
        }

        return logFile;
    }

    /// <summary>
    /// Reads the contents of a LogFile and parses individual LogEntries into the <see cref="LogEntry"/> domain model.
    /// </summary>
    /// <param name="fileResult">The file, as a result of a pick action by the user, and its content type.</param>
    /// <returns>List of parsed log entries.</returns>
    /// <exception cref="IOException">Thrown when the log file could not be read.</exception>
    private async Task<List<LogEntry>> ParseLogEntriesAsync(FileResult fileResult)
    {
        var entries = new List<LogEntry>();
        try
        {
            await using var stream = await fileResult.OpenReadAsync();
            using var reader = new StreamReader(stream);
            var headersParsed = false;
            var headers = new List<string>();

            while (await reader.ReadLineAsync() is { } line)
            {
                if (!headersParsed)
                {
                    headers.AddRange(line.Split('\t'));
                    headersParsed = true;
                    continue;
                }

                if (IsLogEntryStart(line))
                {
                    var entry = ParseLogEntry(line, headers);
                    entry.Hash = hashService.GenerateLogEntryHash(entry);
                    entries.Add(entry);
                }
                else
                {
                    AppendToLastEntryData(entries, line);
                }
            }
        }
        catch (IOException ex)
        {
            logger.LogError("Failed to read file: {ErrorMessage}", ex.Message);
            throw;
        }
        return entries;
    }

    /// <summary>
    /// Determines if a line indicates the start of a new log entry based on date format and header presence.
    /// </summary>
    /// <param name="line">Line to evaluate.</param>
    /// <returns>True if line starts a new log entry; otherwise, false.</returns>
    private static bool IsLogEntryStart(string line)
    {
        var parts = line.Split('\t', 2);
        return DateTime.TryParseExact(parts[0], DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
               && parts.Length == 2;  // Ensure there's more to the line than just a date
    }

    /// <summary>
    /// Appends additional data to the last log entry if not starting a new entry.
    /// </summary>
    /// <param name="entries">Current list of entries.</param>
    /// <param name="line">Line to append.</param>
    private static void AppendToLastEntryData(List<LogEntry> entries, string line)
    {
        if (entries.Count > 0)
        {
            entries[^1].Data += line + Environment.NewLine;
        }
    }

    /// <summary>
    /// Converts a line of text into an <see cref="LogEntry"/> object based on provided headers.
    /// </summary>
    /// <param name="line">Line of text to parse.</param>
    /// <param name="headers">Column headers for mapping data.</param>
    /// <returns>Parsed LogEntry object.</returns>
    private LogEntry ParseLogEntry(string line, IReadOnlyList<string> headers)
    {
        var values = line.Split('\t');
        var entry = new LogEntry();

        for (var i = 0; i < headers.Count && i < values.Length; i++)
        {
            PopulateEntry(entry, headers[i], values[i]);
        }

        return entry;
    }

    /// <summary>
    /// Parses a time string to set the time and tick properties on a LogEntry.
    /// </summary>
    /// <param name="timeString">The string containing the time and optionally ticks.</param>
    /// <param name="entry">The LogEntry to update with parsed time information.</param>
    private static void ParseTime(string timeString, LogEntry entry)
    {
        var parts = timeString.Split(' ');
        if (parts.Length <= 1) return;

        var time = TimeOnly.ParseExact(parts[0], TimeFormat, CultureInfo.InvariantCulture);
        entry.LogTimeStamp.DateTime = entry.LogTimeStamp.DateTime.Add(time.ToTimeSpan());
        var ticks = parts[1];
        entry.LogTimeStamp.Ticks = long.Parse(ticks);
    }

    private static LogType ParseLogType(string logTypeString)
    {
        logTypeString = logTypeString.Trim();
        return logTypeString.ToLower() switch
        {
            "debug" => LogType.Debug,
            "info" => LogType.Information,
            "information" => LogType.Information,
            "warn" => LogType.Warning,
            "warning" => LogType.Warning,
            "error" => LogType.Error,
            "fatal" => LogType.Error,
            _ => throw new ArgumentException($"Unrecognized log type: {logTypeString}")
        };
    }

    /// <summary>
    /// Populates properties of a LogEntry based on header and value, including parsing dates and times.
    /// </summary>
    /// <param name="entry">Log entry to populate.</param>
    /// <param name="header">Header corresponding to the property to populate.</param>
    /// <param name="value">Value to set.</param>
    private void PopulateEntry(LogEntry entry, string header, string value)
    {
        try
        {
            switch (header.ToUpper())
            {
                case "DATE":
                    entry.LogTimeStamp.DateTime = DateTime.ParseExact(value, DateFormat, CultureInfo.InvariantCulture);
                    break;
                case "TIME":
                    ParseTime(value, entry);
                    break;
                case "LOGTYPE":
                    entry.LogType = ParseLogType(value);
                    break;
                case "THREADNAME/NUMBER":
                    entry.ThreadNameOrNumber = value;
                    break;
                case "SOURCELOCATION":
                    entry.SourceLocation = value;
                    break;
                case "SOURCE":
                    entry.Source = value;
                    break;
                case "CATEGORY":
                    entry.Category = value;
                    break;
                case "EVENTID":
                    entry.EventId = int.Parse(value);
                    break;
                case "USER":
                    entry.User = value;
                    break;
                case "COMPUTER":
                    entry.Computer = value;
                    break;
                case "DESCRIPTION":
                    entry.Description = value;
                    break;
                case "DATA":
                    entry.Data = value;
                    break;
                default:
                    logger.LogWarning("Unhandled property for header: {Header}", header);
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Error populating {Header}: {ErrorMessage}", header, ex.Message);
            throw;
        }
    }
}
