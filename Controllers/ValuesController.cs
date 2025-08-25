using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]                          // Bunu ekle
    [Route("api/[controller]")]              // Route tanımı

    public class ValuesController : Controller
    {
        //https://localhost:7081/api/values?YourName=Emine

        [HttpGet]
        public IActionResult Get(string YourName)
        {
            List<string> _data = new List<string>()
            {
                "Hello Ziraat Team from " + YourName
            };

            return Ok(_data);
        }
    }
}
