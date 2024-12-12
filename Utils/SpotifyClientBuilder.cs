using Microsoft.AspNetCore.Authentication;
using SpotifyAPI.Web;

namespace SparkStatsAPI.Utils;

public class SpotifyClientBuilder(
  IHttpContextAccessor accessor,
  SpotifyClientConfig config)
{
  public async Task<(SpotifyClient?, string?)> Build()
  {
    var context = _accessor.HttpContext;
    if (context == null) { return (null, "Can't access HTTP context"); }

    var token = await context
      .GetTokenAsync("Spotify", "access_token");
    if (token == null) { return (null, "Can't fetch access token"); }

    var client = new SpotifyClient(_config.WithToken(token));

    return (client, null);
  }

  private readonly IHttpContextAccessor _accessor = accessor;
  private readonly SpotifyClientConfig _config = config;
}
