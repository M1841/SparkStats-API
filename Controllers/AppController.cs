using Microsoft.AspNetCore.Mvc;

namespace SparkStatsAPI.Controllers;

[Route("/")]
[ApiController]
public class AppController : ControllerBase
{
  [HttpGet]
  public IActionResult Greeting()
  {
    return Ok("Welcome to the new SparkStats!");
  }
}
