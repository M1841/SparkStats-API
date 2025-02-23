using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SparkStatsAPI.Services;

namespace SparkStatsAPI
{
  namespace Controllers
  {
    [Route("[controller]")]
    [ApiController]
    public class GenreController(GenreService genreService) : ControllerBase
    {
      [HttpGet("top")]
      public async Task<IActionResult> GetTop(TimeRange range,
        [FromHeader(Name = "Authorization")] string authHeader)
      {
        var result =
          await genreService.GetTop(range, authHeader);

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
