using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(IConfiguration config, ILogger<ValuesController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(string YourName)
        {
            var message = "Hello Ziraat Team from " + YourName;
            var data = new List<string> { message };

            var connString = ResolveConnectionString();
            if (string.IsNullOrWhiteSpace(connString))
            {
                _logger.LogError("Database connection string could not be resolved.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Veritabani baglantisi yapilandirilamadi." });
            }

            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                using var cmd = new NpgsqlCommand(
                    "INSERT INTO messages (name, text) VALUES (@n, @t)", conn);
                cmd.Parameters.AddWithValue("n", YourName);
                cmd.Parameters.AddWithValue("t", message);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database insert failed.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Veritabani kaydi sirasinda hata olustu." });
            }

            return Ok(data);
        }

        private string? ResolveConnectionString()
        {
            var configured = _config.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(configured))
            {
                var expanded = Environment.ExpandEnvironmentVariables(configured);
                if (!string.IsNullOrWhiteSpace(expanded) && !expanded.Contains("${"))
                {
                    return expanded;
                }
            }

            var host = Environment.GetEnvironmentVariable("POSTGRES_HOST")
                       ?? "postgres.devopschallenge.svc.cluster.local";
            var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
            var user = Environment.GetEnvironmentVariable("POSTGRES_USER");
            var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            var portValue = Environment.GetEnvironmentVariable("POSTGRES_PORT");

            if (string.IsNullOrWhiteSpace(database) ||
                string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var port = 5432;
            if (!string.IsNullOrWhiteSpace(portValue) &&
                int.TryParse(portValue, out var parsedPort))
            {
                port = parsedPort;
            }

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = port,
                Database = database,
                Username = user,
                Password = password
            };

            return builder.ToString();
        }
    }
}
