using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SparkStatsAPI.Extensions;
using SparkStatsAPI.Utils;

namespace SparkStatsAPI.Controllers;

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
      var (spotify, error) = _builder.Build(authHeader);
      if (error != null)
      {
        return StatusCode(
          StatusCodes.Status500InternalServerError,
          error);
      }

      var request = new UsersTopItemsRequest(range)
      { Limit = 50 };
      var response = await spotify!
        .UserProfile.GetTopArtists(request);

      var paging = PagingAdapter<FullArtist>.From(response);

      var artists = new List<ArtistSimple>();
      await foreach (var artist in spotify!.Paginate(paging))
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
    catch (Exception error)
    {
      return StatusCode(
        StatusCodes.Status500InternalServerError,
        error.Message);
    }
  }

  private static string[] SelectGenres(FullArtist artist)
  {
    return artist
      .Genres.Take(3)
      .Select(genre => genre.ToTitleCase())
      .ToArray();
  }
  private readonly SpotifyClientBuilder _builder = builder;
}
