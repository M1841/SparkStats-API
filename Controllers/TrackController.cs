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
  public async Task<IActionResult> GetCurrent()
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

      var item = (await spotify!.Player.GetCurrentlyPlaying(
        new PlayerCurrentlyPlayingRequest(
          PlayerCurrentlyPlayingRequest.AdditionalTypes.Track))).Item;
      if (item.Type != ItemType.Track)
      {
        return NoContent();
      }

      var track = (FullTrack)item;
      var artists = track.Artists.Select(
        artist => new ArtistBase(
          artist.Name,
          artist.ExternalUrls.FirstOrDefault().Value))
        .ToArray();

      return Ok(new TrackSimple(
        track.Name,
        track.ExternalUrls.FirstOrDefault().Value,
        artists,
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
  public async Task<IActionResult> GetHistory()
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

      var response = await spotify!.Player
        .GetRecentlyPlayed(new PlayerRecentlyPlayedRequest());

      var tracks = new List<TrackSimple>();
      await foreach (var item in spotify!.Paginate(response))
      {
        var track = item.Track;
        if (track.Type == ItemType.Track)
        {
          var artists = track.Artists.Select(
            artist => new ArtistBase(
              artist.Name,
              artist.ExternalUrls.FirstOrDefault().Value
            )).ToArray();

          tracks.Add(new TrackSimple(
            track.Name,
            track.ExternalUrls.FirstOrDefault().Value,
            artists,
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

  private readonly SpotifyClientBuilder _builder = builder;
}
