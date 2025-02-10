using Microsoft.AspNetCore.Mvc;

namespace SparkStatsAPI
{
  namespace Controllers
  {
    [Route("/")]
    [ApiController]
    public class AppController : ControllerBase
    {
      [HttpGet]
      public IActionResult Greeting()
      {
        return Ok("Welcome to SparkStats!");
      }
    }
  }
};
