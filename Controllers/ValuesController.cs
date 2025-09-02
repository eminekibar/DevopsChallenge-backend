using Microsoft.AspNetCore.Mvc;
using Npgsql; // eklendi

namespace backend.Controllers
{
    [ApiController]                          // Bunu ekle
    [Route("api/[controller]")]              // Route tanımı

    public class ValuesController : Controller
    {
        private readonly IConfiguration _config;

        public ValuesController(IConfiguration config)
        {
            _config = config;
        }
        //https://localhost:7081/api/values?YourName=Emine

        [HttpGet]
        public IActionResult Get(string YourName)
        {
            var message = "Hello Ziraat Team from " + YourName;   // <-- EKSİK OLAN SATIR

            // Response verin istersen eski listen kalsın:
            var _data = new List<string> { message };

            string? connString = _config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new InvalidOperationException("Database connection string not configured!");
            }

            // DB’ye kaydet
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();
                using var cmd = new NpgsqlCommand(
                    "INSERT INTO messages (name, message) VALUES (@n, @t)", conn);
                cmd.Parameters.AddWithValue("n", YourName);
                cmd.Parameters.AddWithValue("t", message);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // hata olursa logla ama response’u bozma
                Console.WriteLine("DB error: " + ex.Message);
            }

            return Ok(_data);
        }
    }
}
