using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.Persistence.Repositories.LogFiles;
using Microsoft.AspNetCore.Mvc;

namespace EliasLogAnalyzer.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogFileController : ControllerBase
{
    private readonly ILogFileRepository logFileRepository;
    private readonly ILogger<LogFileController> logger;

    public LogFileController(ILogFileRepository logFileRepository, ILogger<LogFileController> logger)
    {
        this.logFileRepository = logFileRepository;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LogFile>>> GetAllLogFiles()
    {
        try
        {
            var files = await logFileRepository.GetAllLogFiles();
            return Ok(files);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get all log files.");
            return StatusCode(500, "An error occurred while retrieving log files.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LogFile>> GetLogFileById(int id)
    {
        try
        {
            var file = await logFileRepository.GetLogFileById(id);
            if (file == null)
            {
                return NotFound();
            }
            return Ok(file);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get log file by ID: {LogFileId}", id);
            return StatusCode(500, "An error occurred while retrieving the log file.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<LogFile>> AddLogFile(LogFile logFile)
    {
        try
        {
            var newFile = await logFileRepository.AddLogFile(logFile);
            return CreatedAtAction(nameof(GetLogFileById), new { id = newFile.LogFileId }, newFile);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add log file.");
            return StatusCode(500, "An error occurred while adding the log file.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLogFile(int id)
    {
        try
        {
            bool result = await logFileRepository.DeleteLogFile(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete log file with ID: {LogFileId}", id);
            return StatusCode(500, "An error occurred while deleting the log file.");
        }
    }
}