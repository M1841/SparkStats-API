using Microsoft.AspNetCore.Mvc;
using SparkStatsAPI.Utils;
using SparkStatsAPI.Services;

namespace SparkStatsAPI
{
  namespace Controllers
  {
    [Route("[controller]")]
    [ApiController]
    public class PlaylistController(PlaylistService playlistService) : ControllerBase
    {
      [HttpGet]
      public async Task<IActionResult> GetAll(
        [FromHeader(Name = "Authorization")] string authHeader)
      {
        var result =
          await playlistService.GetAll(authHeader);

        if (!result.IsSuccess)
        {
          return StatusCode(
            result.Error!.Status,
            result.Error!.Message);
        }
        return Ok(result.Ok!);
      }

      [HttpPost("shuffle")]
      public async Task<IActionResult> Shuffle(
        [FromBody] ShuffleRequest request,
        [FromHeader(Name = "Authorization")] string authHeader)
      {
        var result =
          await playlistService.Shuffle(request, authHeader);

        if (!result.IsSuccess)
        {
          return StatusCode(
            result.Error!.Status,
            result.Error!.Message);
        }
        return Ok(result.Ok!);
      }
    }
  }
}
