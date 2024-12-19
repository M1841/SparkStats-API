using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SparkStatsAPI.Utils;

namespace SparkStatsAPI.Controllers;

[Authorize("Spotify")]
[Route("[controller]")]
[ApiController]
public class TrackController(
  SpotifyClientBuilder builder
) : ControllerBase
{
  [HttpGet("current")]
  public async Task<IActionResult> GetCurrent(
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

      var response = await spotify!.Player.GetCurrentlyPlaying(
        new PlayerCurrentlyPlayingRequest(
          PlayerCurrentlyPlayingRequest.AdditionalTypes.Track));

      var isPlaying = response.IsPlaying;
      var item = response.Item;

      if (!isPlaying || item?.Type != ItemType.Track)
      {
        return NoContent();
      }

      var track = (FullTrack)item;

      return Ok(new TrackSimple(
        track.Name,
        track.ExternalUrls.FirstOrDefault().Value,
        SelectArtists(track),
        track.Album.Images.LastOrDefault()?.Url));
    }
    catch (Exception error)
    {
      return StatusCode(
        StatusCodes.Status500InternalServerError,
        error.Message);
    }
  }

  [HttpGet("history")]
  public async Task<IActionResult> GetHistory(
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

      var response = await spotify!.Player
        .GetRecentlyPlayed(new PlayerRecentlyPlayedRequest());

      var tracks = new List<TrackSimple>();
      await foreach (var item in spotify!.Paginate(response))
      {
        var track = item.Track;
        if (track.Type == ItemType.Track)
        {
          tracks.Add(new TrackSimple(
            track.Name,
            track.ExternalUrls.FirstOrDefault().Value,
            SelectArtists(track),
            track.Album.Images.LastOrDefault()?.Url));
          if (tracks.Count == 50) { break; }
        }
      }

      return Ok(tracks.ToArray());
    }
    catch (Exception error)
    {
      return StatusCode(
        StatusCodes.Status500InternalServerError,
        error.Message);
    }
  }

  [HttpGet("top")]
  public async Task<IActionResult> GetTop(TimeRange range,
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
        .UserProfile.GetTopTracks(request);

      var paging = PagingAdapter<FullTrack>.From(response);

      var tracks = new List<TrackSimple>();
      await foreach (var track in spotify!.Paginate(paging))
      {
        tracks.Add(new TrackSimple(
          track.Name,
          track.ExternalUrls.FirstOrDefault().Value,
          SelectArtists(track),
          track.Album.Images.LastOrDefault()?.Url
        ));
        if (tracks.Count == 100) { break; }
      }

      return Ok(tracks.ToArray());
    }
    catch (Exception error)
    {
      return StatusCode(
        StatusCodes.Status500InternalServerError,
        error.Message);
    }
  }

  private static ArtistBase[] SelectArtists(FullTrack track)
  {
    return track.Artists.Select(
      artist => new ArtistBase(
        artist.Name,
        artist.ExternalUrls.FirstOrDefault().Value
      )).ToArray();
  }

  private readonly SpotifyClientBuilder _builder = builder;
}
