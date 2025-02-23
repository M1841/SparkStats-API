using SpotifyAPI.Web;
using SparkStatsAPI.Extensions;
using SparkStatsAPI.Utils;

namespace SparkStatsAPI
{
  namespace Services
  {
    public class ArtistService(SpotifyClientBuilder builder)
    {
      public async Task<Result<ArtistSimple[]>> GetTop(
        TimeRange range, string authHeader)
      {

        try
        {
          var buildResult = builder.Build(authHeader);
          if (!buildResult.IsSuccess)
          {
            return Result<ArtistSimple[]>.Failure(
              buildResult.Error!.Message,
              buildResult.Error!.Status);
          }
          var spotify = buildResult.Ok!;

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

          return Result<ArtistSimple[]>.Success(artists.ToArray());
        }
        catch (APIUnauthorizedException error)
        {
          return Result<ArtistSimple[]>.Failure(
            error.Message,
            StatusCodes.Status401Unauthorized);
        }
        catch (Exception error)
        {
          return Result<ArtistSimple[]>.Failure(
            error.Message,
            StatusCodes.Status500InternalServerError);
        }
      }
      private static string[] SelectGenres(FullArtist artist)
      {
        return artist
          .Genres.Take(3)
          .Select(genre => genre.ToTitleCase())
          .ToArray();
      }
    }
  }
}
