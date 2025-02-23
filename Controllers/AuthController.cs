using Microsoft.AspNetCore.Mvc;
using SparkStatsAPI.Utils;
using SparkStatsAPI.Services;

namespace SparkStatsAPI
{
  namespace Controllers
  {
    [Route("/auth")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
    {
      [HttpGet("login")]
      public IActionResult Login()
      {
        return Redirect(authService.Login());
      }

      [HttpPost("refresh")]
      public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
      {
        var result = await authService.Refresh(request);
        if (!result.IsSuccess)
        {
          return StatusCode(
            result.Error!.Status,
            result.Error!.Message);
        }
        return Ok(result.Ok!);
      }

      [HttpGet("callback")]
      public async Task<IActionResult> Callback([FromQuery] string code)
      {
        return Redirect(await authService.Callback(code));
      }
    }
  }
}
