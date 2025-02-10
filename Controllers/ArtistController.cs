using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SparkStatsAPI.Extensions;
using SparkStatsAPI.Utils;

namespace SparkStatsAPI
{
  namespace Controllers
  {
    [Route("[controller]")]
    [ApiController]
    public class ArtistController(
      SpotifyClientBuilder builder
    ) : ControllerBase
    {
      [HttpGet("top")]
      public async Task<IActionResult> GetTop(
        [FromQuery] TimeRange range,
        [FromHeader(Name = "Authorization")] string authHeader)
      {
        try
        {
          var result = _builder.Build(authHeader);
          if (!result.IsSuccess)
          {
            return StatusCode(
              result.Error!.Status,
              result.Error.Message);
          }
          SpotifyClient? spotify = result.Ok!;

          var request = new UsersTopItemsRequest(range)
          { Limit = 50 };
          var response = await spotify
            .UserProfile.GetTopArtists(request);

          var paging = PagingAdapter.ArtistPages(response);

          var artists = new List<ArtistSimple>();
          await foreach (var artist in spotify.Paginate(paging))
          {
            artists.Add(new ArtistSimple(
              artist.Id,
              artist.Name,
              artist.ExternalUrls.FirstOrDefault().Value,
              artist.Images.LastOrDefault()?.Url,
              SelectGenres(artist)
            ));
            if (artists.Count == 100) { break; }
          }

          return Ok(artists.ToArray());
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

      private static string[] SelectGenres(FullArtist artist)
      {
        return [.. artist
          .Genres.Take(3)
          .Select(genre => genre.ToTitleCase())];
      }
      private readonly SpotifyClientBuilder _builder = builder;
    }
  }
}
