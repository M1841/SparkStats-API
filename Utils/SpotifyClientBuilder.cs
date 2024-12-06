using Microsoft.AspNetCore.Authentication;
using SpotifyAPI.Web;

namespace SparkStatsAPI.Utils;

public class SpotifyClientBuilder(
  IHttpContextAccessor httpContextAccessor,
  SpotifyClientConfig spotifyClientConfig)
{
  public async Task<SpotifyClient> BuildClient()
  {
    var token = await _httpContextAccessor.HttpContext!
      .GetTokenAsync("Spotify", "access_token");

    return new SpotifyClient(_spotifyClientConfig.WithToken(token!));
  }

  private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
  private readonly SpotifyClientConfig _spotifyClientConfig = spotifyClientConfig;
}
