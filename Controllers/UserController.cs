using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SparkStatsAPI.Utils;

namespace SparkStatsAPI.Controllers;

[Authorize("Spotify")]
[Route("[controller]")]
[ApiController]
public class UserController(
  SpotifyClientBuilder spotifyClientBuilder
) : ControllerBase
{
  [HttpGet("profile")]
  public async Task<IActionResult> GetProfile()
  {
    var spotify = await _spotifyClientBuilder.BuildClient();
    var profile = await spotify.UserProfile.Current();

    return Ok(profile);
  }

  [HttpGet("signout")]
  public async Task<IActionResult> LogOut()
  {
    await HttpContext.SignOutAsync();
    return Redirect("/");
  }

  private readonly SpotifyClientBuilder
    _spotifyClientBuilder = spotifyClientBuilder;
}
