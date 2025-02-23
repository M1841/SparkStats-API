using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SparkStatsAPI.Services;

namespace SparkStatsAPI
{
  namespace Controllers
  {
    [Route("[controller]")]
    [ApiController]
    public class TrackController(TrackService trackService) : ControllerBase
    {
      [HttpGet("current")]
      public async Task<IActionResult> GetCurrent(
        [FromHeader(Name = "Authorization")] string authHeader)
      {
        var result = await trackService.GetCurrent(authHeader);

        if (!result.IsSuccess)
        {
          return StatusCode(
            result.Error!.Status,
            result.Error!.Message);
        }
        return Ok(result.Ok!);
      }

      [HttpGet("history")]
      public async Task<IActionResult> GetHistory(
        [FromHeader(Name = "Authorization")] string authHeader)
      {
        var result = await trackService.GetHistory(authHeader);

        if (!result.IsSuccess)
        {
          return StatusCode(
            result.Error!.Status,
            result.Error!.Message);
        }
        return Ok(result.Ok!);
      }

      [HttpGet("top")]
      public async Task<IActionResult> GetTop(TimeRange range,
        [FromHeader(Name = "Authorization")] string authHeader)
      {
        var result = await trackService.GetTop(range, authHeader);

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
