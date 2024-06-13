using Microsoft.AspNetCore.Mvc;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.Persistence.Repositories.BugReports;

namespace EliasLogAnalyzer.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BugReportController : ControllerBase
{
    private readonly IBugReportRepository bugReportRepository;
    private readonly ILogger<BugReportController> logger;

    public BugReportController(IBugReportRepository bugReportRepository, ILogger<BugReportController> logger)
    {
        this.bugReportRepository = bugReportRepository;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BugReport>>> GetAllBugReports()
    {
        try
        {
            var reports = await bugReportRepository.GetAllBugReports();
            return Ok(reports);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get all bug reports.");
            return StatusCode(500, "An error occurred while retrieving bug reports.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BugReport>> GetBugReportById(int id)
    {
        try
        {
            var report = await bugReportRepository.GetBugReportById(id);
            if (report == null)
            {
                return NotFound();
            }
            return Ok(report);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get bug report by ID: {BugReportId}", id);
            return StatusCode(500, "An error occurred while retrieving the bug report.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<BugReport>> AddBugReport(BugReport bugReport)
    {
        try
        {
            var newReport = await bugReportRepository.AddBugReportWithEntries(bugReport);
            return CreatedAtAction(nameof(GetBugReportById), new { id = newReport.BugReportId }, newReport);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add new bug report.");
            return StatusCode(500, "An error occurred while adding the bug report.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBugReport(int id)
    {
        try
        {
            bool result = await bugReportRepository.DeleteBugReport(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete bug report with ID: {BugReportId}", id);
            return StatusCode(500, "An error occurred while deleting the bug report.");
        }
    }
}
