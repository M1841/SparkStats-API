using Microsoft.AspNetCore.Mvc;
using SparkStatsAPI.Utils;
using SparkStatsAPI.Extensions;
using SpotifyAPI.Web;

namespace SparkStatsAPI
{
  namespace Controllers
  {
    [Route("[controller]")]
    [ApiController]
    public class GenreController(
      SpotifyClientBuilder builder
    ) : ControllerBase
    {
      [HttpGet("top")]
      public async Task<IActionResult> GetTop(TimeRange range,
        [FromHeader(Name = "Authorization")] string authHeader)
      {
        try
        {
          var buildResult = _buider.Build(authHeader);
          if (!buildResult.IsSuccess)
          {
            return StatusCode(
              buildResult.Error!.Status,
              buildResult.Error.Message);
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

          return Ok(genres.OrderByDescending(
            (genre) => genre.Value));
        }
        catch (APIUnauthorizedException error)
        {
          return Unauthorized(error.Message);
        }
        catch (Exception error)
        {
          return StatusCode(
            StatusCodes.Status500InternalServerError,
            error.Message);
        }
      }
      private readonly SpotifyClientBuilder _buider = builder;
    }
  }
}
