using System.Globalization;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace EliasLogAnalyzer.MAUI.Services;

/// <summary>
/// Class for reading a LogFile and parsing individual LogEntries into the <see cref="LogEntry"/> domain model.
/// </summary>
public class LogFileParserService : ILogFileParserService
{
    private readonly ILogger<LogFileParserService> _logger;
    private readonly List<string> _headers = [];
    private const string DateFormat = "dd-MM-yyyy";
    private const string TimeFormat = "HH:mm:ss";

    public LogFileParserService(ILogger<LogFileParserService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Reads the contents of a LogFile and parses individual LogEntries into the <see cref="LogEntry"/> domain model.
    /// </summary>
    /// <param name="fileResult">The representation of a file, as a result of a pick action by the user, and its content type.</param>
    /// /// <exception cref="IOException">Thrown when the log file could not be read.</exception>
    public async Task<(List<LogEntry> Entries, long FileSize)> ParseLogAsync(FileResult fileResult)
    {
        _logger.LogInformation("Starting to parse log file: {FileName}", fileResult.FileName);
        var headers = new List<string>(); // Local headers list for each file

        var entries = new List<LogEntry>();
        long fileSize = 0;

        try
        {
            await using (var stream = await fileResult.OpenReadAsync())
            {
                // Checking if the stream supports seeking to obtain file size
                if (stream.CanSeek)
                {
                    fileSize = stream.Length;
                    _logger.LogInformation("File size: {FileSize} bytes", fileSize);
                }

                using (var reader = new StreamReader(stream))
                {
                    var line = await reader.ReadLineAsync();
                    if (line != null)
                    {
                        // Parse headers using the local list
                        ParseHeaders(line, headers);
                        await ParseEntries(reader, entries, headers);
                    }
                }
            }

            LogEntries(entries);
        }
        catch (IOException ex)
        {
            _logger.LogError("An I/O error occurred while reading the file: {ErrorMessage}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred: {ErrorMessage}", ex.Message);
        }

        return (entries, fileSize);
    }

    /// <summary>
    /// The first log line of a LogFile must be the header. Reads and extracts headers from the first line of the file.
    /// </summary>
    /// <param name="line">The first line of a LogFile which holds the headers.</param>
    private void ParseHeaders(string line, List<string> headers)
    {
        try
        {
            headers.AddRange(line.Split('\t'));
            _logger.LogInformation("Headers read from file: {Headers}", string.Join(", ", headers));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error parsing headers: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Reads and Parses lines in a LogFile and checks if they are new log entries or continuation of the previous entry.
    /// </summary>
    /// <param name="reader">The Streamreader.</param>
    /// <param name="entries">The list of LogEntries for this LogFile.</param>
    private async Task ParseEntries(StreamReader reader, List<LogEntry> entries, List<string> headers)
    {
        while (await reader.ReadLineAsync() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            try
            {
                if (IsLogEntryStart(line, headers))
                {
                    entries.Add(ParseLogEntry(line, headers));
                }
                else
                {
                    AppendToLastEntryData(entries, line);
                }
            }
            catch (FormatException ex)
            {
                _logger.LogError("Format error while parsing line: {LineContent} - {ErrorMessage}", line, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing line: {LineContent} - {ErrorMessage}", line, ex.Message);
            }
        }
    }

    /// <summary>
    /// Check if the line starts with a date that matches the "Date" field format and has the same amount of fields as
    /// the headers minus Data, to determine a new log entry start.
    /// </summary>
    /// <param name="line">The line to check if it matches the new LogEntry signature.</param>
    private bool IsLogEntryStart(string line, List<string> headers)
    {
        var splitLine = line.Split('\t');
        return DateTime.TryParseExact(splitLine[0], DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
            out _) && splitLine.Length >= headers.Count - 1;
    }

    /// <summary>
    /// Appends a line to the Data field of the last log entry in the list.
    /// </summary>
    private void AppendToLastEntryData(List<LogEntry> entries, string line)
    {
        if (entries.Count > 0)
        {
            entries[^1].Data += line + Environment.NewLine;
        }
    }

    /// <summary>
    /// Creates a new <see cref="LogEntry"/> class instance and populates it with values from the new LogEntry line.
    /// </summary>
    /// <param name="line">The line that holds the values of a new LogEntry</param>
    private LogEntry ParseLogEntry(string line, List<string> headers)
    {
        var values = line.Split('\t');
        var entry = new LogEntry();

        for (var i = 0; i < headers.Count && i < values.Length; i++)
        {
            var header = headers[i];
            var value = values[i];
            PopulateEntry(entry, header, value);
        }

        return entry;
    }
    
    private LogType ParseLogType(string logTypeString)
    {
        logTypeString = logTypeString.Trim();

        // Normalize case variations
        switch (logTypeString.ToLower())
        {
            case "debug":
                return LogType.Debug;
            case "info":
            case "information":
                return LogType.Information;
            case "warn":
            case "warning":
                return LogType.Warning;
            case "error":
            case "fatal":
                return LogType.Error; 
            default:
                _logger.LogWarning("Unrecognized log type '{LogType}'", logTypeString);
                return LogType.None;
        }
    }

    private void ParseTime(string timeString, LogEntry entry)
    {
        var parts = timeString.Split(' ');
        if (parts.Length > 1)
        {
            var time = TimeOnly.ParseExact(parts[0], TimeFormat, CultureInfo.InvariantCulture);
            entry.LogTimeStamp.DateTime = entry.LogTimeStamp.DateTime.Add(time.ToTimeSpan());
            entry.LogTimeStamp.Ticks = parts[1];
        }
    }

    /// <summary>
    /// Determines the header and value of a line and populates the corresponding field in the <see cref="LogEntry"/> class.
    /// </summary>
    /// <param name="entry">The <see cref="LogEntry"/>  class to populate.</param>
    /// <param name="header">The header which corresponds to a field of the see cref="LogEntry"/> class.</param>
    /// <param name="value">The value which belongs to the header.</param>
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
                case "DATETIMESORTVALUE":
                    entry.LogTimeStamp.DateTimeSortValue = value;
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
                    // Set Data to empty initially, to be filled if there are subsequent lines by AppendToLastEntryData()
                    entry.Data = "";
                    break;
            }
        }
        catch (FormatException ex)
        {
            _logger.LogError("Format error parsing {Header} with value {Value}: {ErrorMessage}", header, value,
                ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error populating {Header} for log entry: {ErrorMessage}", header, ex.Message);
            throw;
        }
    }

    // Used during development to log the parsed entries to the console
    private void LogEntries(List<LogEntry> entries)
    {
        foreach (var entry in entries)
        {
            _logger.LogInformation("Log entry: {LogEntry}", entry.ToString());
        }
    }
}