using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SparkStatsAPI.Utils;
using SpotifyAPI.Web;

namespace SparkStatsAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController(
  SpotifyClientBuilder builder
) : ControllerBase
{
  [HttpGet("profile")]
  public async Task<IActionResult> GetProfile(
    [FromHeader(Name = "Authorization")] string authHeader)
  {
    try
    {
      var result = _builder.Build(authHeader);
      if (!result.IsSuccess)
      {
        return StatusCode(
          result.Error!.Status,
          result.Error.Message);
      }
      var spotify = result.Ok!;

      var profile = await spotify.UserProfile.Current();

      return Ok(new UserProfileSimple(
        profile.Id,
        profile.DisplayName,
        profile.ExternalUrls.FirstOrDefault().Value,
        profile.Images.LastOrDefault()?.Url));
    }
    catch (APIUnauthorizedException error)
    {
      return Unauthorized(error.Message);
    }
    catch (Exception error)
    {
      return StatusCode(
        StatusCodes.Status500InternalServerError,
        error.Message);
    }
  }

  [HttpGet("signout")]
  public async Task<IActionResult> LogOut()
  {
    await HttpContext.SignOutAsync();
    return Redirect("/");
  }

  private readonly SpotifyClientBuilder _builder = builder;
}
