using Microsoft.AspNetCore.Mvc;
using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.API.Repositories.LogEntries;

namespace EliasLogAnalyzer.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogEntryController : ControllerBase
{
    private readonly ILogEntryRepository logEntryRepository;
    private readonly ILogger<LogEntryController> logger;

    public LogEntryController(ILogEntryRepository logEntryRepository, ILogger<LogEntryController> logger)
    {
        this.logEntryRepository = logEntryRepository;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LogEntry>>> GetLogEntries()
    {
        try
        {
            var entries = await logEntryRepository.GetLogEntries();
            return Ok(entries);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve log entries.");
            return StatusCode(500, "An error occurred while retrieving log entries.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LogEntry>> GetLogEntryById(int id)
    {
        try
        {
            var logEntry = await logEntryRepository.GetLogEntryById(id);
            if (logEntry == null)
            {
                return NotFound();
            }
            return Ok(logEntry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve log entry by ID: {LogEntryId}", id);
            return StatusCode(500, "An error occurred while retrieving the log entry.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<LogEntry>> AddLogEntry(LogEntry logEntry)
    {
        try
        {
            var addedEntry = await logEntryRepository.AddOrUpdateLogEntry(logEntry);
            return CreatedAtAction(nameof(GetLogEntryById), new { id = addedEntry.LogEntryId }, addedEntry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add log entry.");
            return StatusCode(500, "An error occurred while adding the log entry.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLogEntry(int id)
    {
        try
        {
            bool result = await logEntryRepository.DeleteLogEntry(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete log entry with ID: {LogEntryId}", id);
            return StatusCode(500, "An error occurred while deleting the log entry.");
        }
    }
}
