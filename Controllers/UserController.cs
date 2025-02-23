using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SparkStatsAPI.Services;

namespace SparkStatsAPI
{
  namespace Controllers
  {
    [Route("[controller]")]
    [ApiController]
    public class UserController(UserService userService) : ControllerBase
    {
      [HttpGet("profile")]
      public async Task<IActionResult> GetProfile(
        [FromHeader(Name = "Authorization")] string authHeader)
      {
        var result =
          await userService.GetProfile(authHeader);

        if (!result.IsSuccess)
        {
          return StatusCode(
            result.Error!.Status,
            result.Error!.Message);
        }
        return Ok(result.Ok!);
      }

      [HttpGet("signout")]
      public async Task<IActionResult> LogOut()
      {
        await HttpContext.SignOutAsync();
        return Redirect("/");
      }
    }
  }
}
