using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SparkStatsAPI.Services;

namespace SparkStatsAPI.Controllers;

[Authorize("Spotify")]
[Route("/")]
[ApiController]
public class AppController(
  SpotifyClientBuilder spotifyClientBuilder) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetProfile()
  {
    var spotify = await _spotifyClientBuilder.BuildClient();
    var me = await spotify.UserProfile.Current();

    return Ok(new
    {
      me.DisplayName,
      me.Email,
      me.Uri
    });
  }

  [HttpPost("/signout")]
  public async Task<IActionResult> LogOut()
  {
    await HttpContext.SignOutAsync();
    return Redirect("https://open.spotify.com/");
  }

  private readonly SpotifyClientBuilder _spotifyClientBuilder = spotifyClientBuilder;
}
