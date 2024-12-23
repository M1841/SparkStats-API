using SpotifyAPI.Web;

namespace SparkStatsAPI.Utils;

public class SpotifyClientBuilder(
  SpotifyClientConfig config)
{
  public (SpotifyClient?, string?) Build(string authHeader)
  {
    if (string.IsNullOrEmpty(authHeader)
      || !authHeader.StartsWith("Bearer "))
    {
      return (null, "Invalid authorization header");
    }
    var token = authHeader["Bearer ".Length..];

    var client = new SpotifyClient(
      _config.WithToken(token));

    return (client, null);
  }

  private readonly SpotifyClientConfig _config = config;
}
