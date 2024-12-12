using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SparkStatsAPI.Utils;

namespace SparkStatsAPI.Controllers;

[Authorize("Spotify")]
[Route("[controller]")]
[ApiController]
public class UserController(
  SpotifyClientBuilder builder
) : ControllerBase
{
  [HttpGet("profile")]
  public async Task<IActionResult> GetProfile()
  {
    try
    {
      var (spotify, error) = await _builder.Build();
      if (error != null)
      {
        return StatusCode(
          StatusCodes.Status500InternalServerError,
          error);
      }

      var profile = await spotify!.UserProfile.Current();

      return Ok(new Profile(
        profile.DisplayName,
        profile.ExternalUrls.FirstOrDefault().Value,
        profile.Images.LastOrDefault()?.Url));
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
