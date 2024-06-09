using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace EliasLogAnalyzer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController(IConfiguration configuration) : ControllerBase
{
    [HttpGet("CheckConnection")]
    public IActionResult CheckDatabaseConnection()
    {
        try
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                return Problem("No connection string configured.", statusCode: 500);
            }

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                return Ok("Connection successful.");
            }
            else
            {
                return Problem("Connection opened but the state is not open.", statusCode: 500);
            }
        }
        catch (SqlException ex)
        {
            return Problem($"SQL error occurred: {ex.Message}", statusCode: 503);
        }
        catch (Exception ex)
        {
            return Problem($"An unexpected error occurred: {ex.Message}", statusCode: 500);
        }
    }

}