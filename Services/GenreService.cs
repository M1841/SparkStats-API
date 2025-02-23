using SpotifyAPI.Web;
using SparkStatsAPI.Extensions;
using SparkStatsAPI.Utils;

namespace SparkStatsAPI
{
  namespace Services
  {
    public class GenreService(SpotifyClientBuilder builder)
    {
      public async Task<Result<GenreSimple[]>> GetTop(TimeRange range, string authHeader)
      {
        try
        {
          var buildResult = builder.Build(authHeader);
          if (!buildResult.IsSuccess)
          {
            return Result<GenreSimple[]>.Failure(
              buildResult.Error!.Message,
              buildResult.Error!.Status);
          }
          var spotify = buildResult.Ok!;

          var request = new UsersTopItemsRequest(range)
          { Limit = 50 };
          var response = await spotify.UserProfile.GetTopArtists(request);

          var paging = PagingAdapter.ArtistPages(response);

          var genres = new Dictionary<string, int>();
          await foreach (var artist in spotify.Paginate(paging))
          {
            foreach (var genre in artist.Genres)
            {
              var genreTitleCase = genre.ToTitleCase();
              if (!genres.TryAdd(genreTitleCase, 1))
              {
                genres[genreTitleCase]++;
              }
            }
            if (genres.Count == 100) { break; }
          }

          return Result<GenreSimple[]>.Success(genres
            .OrderByDescending(genre
              => genre.Value)
            .Select(genre
              => new GenreSimple(
                genre.Key,
                genre.Value))
            .ToArray());
        }
        catch (APIUnauthorizedException error)
        {
          return Result<GenreSimple[]>.Failure(
            error.Message,
            StatusCodes.Status401Unauthorized);
        }
        catch (Exception error)
        {
          return Result<GenreSimple[]>.Failure(
            error.Message,
            StatusCodes.Status500InternalServerError);
        }
      }
    }
  }
}
