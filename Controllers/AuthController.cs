using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

namespace SparkStatsAPI.Controllers;

[Route("/auth")]
[ApiController]
public class AuthController : ControllerBase
{
  [HttpGet("login")]
  public IActionResult Login()
  {
    var server = new EmbedIOAuthServer(
      new Uri($"{_backendUrl}/auth/callback"),
      _port);
    var request = new LoginRequest(
      server.BaseUri,
      _clientId,
      LoginRequest.ResponseType.Code)
    {
      Scope = [
        Scopes.PlaylistModifyPrivate,
        Scopes.PlaylistModifyPublic,
        Scopes.PlaylistReadCollaborative,
        Scopes.PlaylistReadPrivate,
        Scopes.UserReadCurrentlyPlaying,
        Scopes.UserReadEmail,
        Scopes.UserReadPrivate,
        Scopes.UserReadRecentlyPlayed,
        Scopes.UserTopRead]
    };

    return Redirect(request.ToUri().ToString());
  }

  [HttpGet("callback")]
  public async Task<IActionResult> Callback([FromQuery] string code)
  {
    var response = await new OAuthClient().RequestToken(
      new AuthorizationCodeTokenRequest(
      _clientId,
      _clientSecret,
      code,
      new Uri($"{_backendUrl}/auth/callback")));

    return Redirect($"{_frontendUrl}/auth-callback?access_token={response.AccessToken}&refresh_token={response.RefreshToken}");
  }

  private readonly string _clientId = Environment
    .GetEnvironmentVariable("SPOTIFY_CLIENT_ID")!;
  private readonly string _clientSecret = Environment
    .GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET")!;
  private readonly int _port = int.Parse(Environment
    .GetEnvironmentVariable("PORT")!);
  private readonly string _frontendUrl = Environment
    .GetEnvironmentVariable("FRONTEND_URL")!;
  private readonly string _backendUrl = Environment
    .GetEnvironmentVariable("BACKEND_URL")!;
}
