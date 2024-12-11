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
      var (spotify, error) = await _builder.BuildClient();
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
        artist => new Artist(
          artist.Name,
          artist.ExternalUrls.FirstOrDefault().Value))
        .ToArray();

      return Ok(new Track(
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
      var (spotify, error) = await _builder.BuildClient();
      if (error != null)
      {
        return StatusCode(
          StatusCodes.Status500InternalServerError,
          error);
      }

      var history = await spotify!.Player
        .GetRecentlyPlayed(new PlayerRecentlyPlayedRequest());

      var tracks = new List<Track>();
      await foreach (var item in spotify!.Paginate(history))
      {
        var track = item.Track;
        if (track.Type == ItemType.Track)
        {
          var artists = track.Artists.Select(
            artist => new Artist(
              artist.Name,
              artist.ExternalUrls.FirstOrDefault().Value
            )).ToArray();

          tracks.Add(new Track(
            track.Name,
            track.ExternalUrls.FirstOrDefault().Value,
            artists,
            track.Album.Images.LastOrDefault()?.Url));
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
