using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SparkStatsAPI.Extensions;
using SparkStatsAPI.Utils;
using SpotifyAPI.Web.Http;

namespace SparkStatsAPI.Controllers;

[Authorize("Spotify")]
[Route("[controller]")]
[ApiController]
public class ArtistController(
  SpotifyClientBuilder builder
) : ControllerBase
{
  [HttpGet("top")]
  public async Task<IActionResult> GetTop(TimeRange range)
  {
    try
    {
      var (spotify, error) = await _builder.Build();
      if (error != null)
      {
        return StatusCode(
          StatusCodes.Status500InternalServerError,
          error);
      }

      var response = await spotify!.UserProfile.GetTopArtists(
        new UsersTopItemsRequest(range));

      var paging = PagingAdapter.From<FullArtist>(response);

      var artists = new List<ArtistSimple>();
      await foreach (var artist in spotify!.Paginate(paging))
      {
        artists.Add(new ArtistSimple(
          artist.Name,
          artist.ExternalUrls.FirstOrDefault().Value,
          SelectGenres(artist),
          artist.Images.LastOrDefault()?.Url
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
