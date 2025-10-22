using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace backend.Controllers
{
    [ApiController] //Denetleyici sınıfının bir API denetleyicisi olduğunu belirtir.
    [Route("api/[controller]")] //Denetleyicinin URL yolunu tanımlar. [controller] yer tutucusu, denetleyici sınıfının adını temsil eder.

    public class ValuesController : Controller //Denetleyici sınıfı
    {
        private readonly IConfiguration _config; //Yapılandırma ayarlarını tutmak için kullanılan özel alan.

        public ValuesController(IConfiguration config) //Yapılandırma ayarlarını alır ve özel alana atar.
        {
            _config = config;
        }

        [HttpGet] //HTTP GET isteklerini işlemek için kullanılır.
        public IActionResult Get(string YourName) //Get metodunu tanımlar ve bir parametre alır.
        {
            var message = "Hello Ziraat Team from " + YourName;

            var _data = new List<string> { message }; 

            string? connString = _config.GetConnectionString("DefaultConnection"); //Yapılandırma dosyasından veritabanı bağlantı dizesini alır.

            if (string.IsNullOrWhiteSpace(connString)) //Bağlantı dizesinin boş olup olmadığını kontrol eder.
            {
                throw new InvalidOperationException("Database connection string not configured!"); //Boşsa bir hata fırlatır.
            }

            try //Veritabanı işlemlerini gerçekleştirmek için try bloğu.
            {
                using var conn = new NpgsqlConnection(connString); //NpgsqlConnection nesnesi oluşturur.
                conn.Open(); //Veritabanı bağlantısını açar.
                using var cmd = new NpgsqlCommand("INSERT INTO messages (name, text) VALUES (@n, @t)", conn); //SQL komutunu hazırlar.
                cmd.Parameters.AddWithValue("n", YourName); //Parametreleri ekler.
                cmd.Parameters.AddWithValue("t", message); 
                cmd.ExecuteNonQuery(); //SQL komutunu çalıştırır.
            }
            catch (Exception ex) //Hata durumunda yakalar.
            {
                Console.WriteLine("DB error: " + ex.Message);
            }

            return Ok(_data); //HTTP 200 OK yanıtı döner ve verileri içerir.
        }
    }
}
