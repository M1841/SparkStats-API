using SpotifyAPI.Web;

namespace SparkStatsAPI
{
  namespace Utils
  {
    public class SpotifyClientBuilder(
      SpotifyClientConfig config)
    {
      public Result<SpotifyClient> Build(string authHeader)
      {
        if (string.IsNullOrEmpty(authHeader)
          || !authHeader.StartsWith("Bearer "))
        {
          return Result<SpotifyClient>.Failure(
            "Invalid authorization header",
            StatusCodes.Status400BadRequest);
        }
        var token = authHeader["Bearer ".Length..];

        var client = new SpotifyClient(
          _config.WithToken(token));

        return Result<SpotifyClient>.Success(client);
      }

      private readonly SpotifyClientConfig _config = config;
    }
  }
}
