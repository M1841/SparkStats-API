using SparkStatsAPI.Utils;
using SpotifyAPI.Web;

namespace SparkStatsAPI
{
  namespace Services
  {
    public class UserService(SpotifyClientBuilder builder)
    {
      public async Task<Result<UserProfileSimple>> GetProfile(string authHeader)
      {
        try
        {
          var buildResult = builder.Build(authHeader);
          if (!buildResult.IsSuccess)
          {
            return Result<UserProfileSimple>.Failure(
              buildResult.Error!.Message,
              buildResult.Error!.Status);
          }
          var spotify = buildResult.Ok!;

          var profile = await spotify.UserProfile.Current();

          return Result<UserProfileSimple>.Success(
            new UserProfileSimple(
              profile.Id,
              profile.DisplayName,
              profile.ExternalUrls.FirstOrDefault().Value,
              profile.Images.LastOrDefault()?.Url
            ));
        }
        catch (APIUnauthorizedException error)
        {
          return Result<UserProfileSimple>.Failure(
            error.Message,
            StatusCodes.Status401Unauthorized);
        }
        catch (Exception error)
        {
          return Result<UserProfileSimple>.Failure(
            error.Message,
            StatusCodes.Status500InternalServerError);
        }
      }
    }
  }
}
