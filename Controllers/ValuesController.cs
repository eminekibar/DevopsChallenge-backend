using Microsoft.AspNetCore.Mvc;
using Npgsql; // eklendi

namespace backend.Controllers
{
    [ApiController]                          // Bunu ekle
    [Route("api/[controller]")]              // Route tanımı

    public class ValuesController : Controller
    {
        private readonly IConfiguration _config;
        public ValuesController(IConfiguration config) => _config = config;

        // Basit selamlama (DB'ye yazmaz)
        // GET /api/values?YourName=Emine
        [HttpGet]
        public IActionResult Get([FromQuery] string YourName)
        {
            var message = "Hello Ziraat Team from " + YourName;
            return Ok(new[] { message });
        }

        public record Msg(string Name, string Message);

        // INSERT endpoint'i
        // POST /api/values   body: { "name": "emine", "message": "pipelines rock" }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Msg dto)
        {
            var connString = _config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connString))
                return StatusCode(500, "Database connection string not configured!");

            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                // DİKKAT: sütun adı message
                const string sql = @"INSERT INTO messages(name, message) VALUES (@name, @message);";
                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("name", dto.Name);
                cmd.Parameters.AddWithValue("message", dto.Message);
                await cmd.ExecuteNonQueryAsync();

                return Ok(new { inserted = true });
            }
            catch (PostgresException pgx)
            {
                // kubectl logs'ta görünür
                Console.WriteLine($"DB error: {pgx.SqlState}: {pgx.MessageText}\nPOSITION: {pgx.Position}");
                return StatusCode(500, "DB error");
            }
        }
    }
}
