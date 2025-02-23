using SparkStatsAPI.Utils;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using Swan;

namespace SparkStatsAPI
{
  namespace Services
  {
    public class AuthService
    {
      public string Login()
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

        return request.ToUri().ToString();
      }

      public async Task<Result<RefreshResponse>> Refresh(RefreshRequest request)
      {
        try
        {
          var response = await new OAuthClient().RequestToken(
            new AuthorizationCodeRefreshRequest(
              _clientId,
              _clientSecret,
              request.RefreshToken));

          var expiresAt = DateTime.UtcNow.AddSeconds(response.ExpiresIn).ToUnixEpochDate() * 1000;
          return Result<RefreshResponse>.Success(new RefreshResponse(
            response.AccessToken,
            expiresAt));
        }
        catch (Exception error)
        {
          return Result<RefreshResponse>.Failure(
            error.Message,
            StatusCodes.Status500InternalServerError);
        }
      }

      public async Task<string> Callback(string code)
      {
        var response = await new OAuthClient().RequestToken(
          new AuthorizationCodeTokenRequest(
          _clientId,
          _clientSecret,
          code,
          new Uri($"{_backendUrl}/auth/callback")));

        var expiresAt = DateTime.UtcNow.AddSeconds(response.ExpiresIn).ToUnixEpochDate();
        return $"{_frontendUrl}/auth-callback?access_token={response.AccessToken}&refresh_token={response.RefreshToken}&expires_at={expiresAt}";
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
  }
}
