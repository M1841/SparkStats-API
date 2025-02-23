using SparkStatsAPI.Utils;
using SpotifyAPI.Web;

namespace SparkStatsAPI
{
  namespace Services
  {
    public class TrackService(SpotifyClientBuilder builder)
    {
      public async Task<Result<TrackSimple>> GetCurrent(string authHeader)
      {
        try
        {
          var buildResult = builder.Build(authHeader);
          if (!buildResult.IsSuccess)
          {
            return Result<TrackSimple>.Failure(
              buildResult.Error!.Message,
              buildResult.Error!.Status);
          }
          var spotify = buildResult.Ok!;

          var response = await spotify.Player.GetCurrentlyPlaying(
            new PlayerCurrentlyPlayingRequest(
              PlayerCurrentlyPlayingRequest.AdditionalTypes.Track));

          var isPlaying = response?.IsPlaying ?? false;
          var item = response?.Item ?? null;

          if (!isPlaying || item?.Type != ItemType.Track)
          {
            return Result<TrackSimple>.Failure(
              "", StatusCodes.Status204NoContent);
          }

          var track = (FullTrack)item;

          return Result<TrackSimple>.Success(new TrackSimple(
            track.Id,
            track.Name,
            track.ExternalUrls.FirstOrDefault().Value,
            track.Album.Images.LastOrDefault()?.Url,
            SelectArtists(track)));

        }
        catch (APIUnauthorizedException error)
        {
          return Result<TrackSimple>.Failure(
            error.Message,
            StatusCodes.Status401Unauthorized);
        }
        catch (Exception error)
        {
          return Result<TrackSimple>.Failure(
            error.Message,
            StatusCodes.Status500InternalServerError);
        }
      }

      public async Task<Result<TrackSimple[]>> GetHistory(string authHeader)
      {
        try
        {
          var buildResult = builder.Build(authHeader);
          if (!buildResult.IsSuccess)
          {
            return Result<TrackSimple[]>.Failure(
              buildResult.Error!.Message,
              buildResult.Error!.Status);
          }
          var spotify = buildResult.Ok!;

          var response = await spotify.Player
            .GetRecentlyPlayed(new PlayerRecentlyPlayedRequest());

          var tracks = new List<TrackSimple>();
          await foreach (var item in spotify.Paginate(response))
          {
            var track = item.Track;
            if (track.Type == ItemType.Track)
            {
              tracks.Add(new TrackSimple(
                track.Id,
                track.Name,
                track.ExternalUrls.FirstOrDefault().Value,
                track.Album.Images.LastOrDefault()?.Url,
                SelectArtists(track)));
              if (tracks.Count == 50) { break; }
            }
          }

          return Result<TrackSimple[]>.Success(tracks.ToArray());
        }
        catch (APIUnauthorizedException error)
        {
          return Result<TrackSimple[]>.Failure(
            error.Message,
            StatusCodes.Status401Unauthorized);
        }
        catch (Exception error)
        {
          return Result<TrackSimple[]>.Failure(
            error.Message,
            StatusCodes.Status500InternalServerError);
        }
      }

      public async Task<Result<TrackSimple[]>> GetTop(TimeRange range, string authHeader)
      {
        try
        {
          var buildResult = builder.Build(authHeader);
          if (!buildResult.IsSuccess)
          {
            return Result<TrackSimple[]>.Failure(
              buildResult.Error!.Message,
              buildResult.Error!.Status);
          }
          var spotify = buildResult.Ok!;

          var request = new UsersTopItemsRequest(range)
          { Limit = 50 };
          var response = await spotify
            .UserProfile.GetTopTracks(request);

          var paging = PagingAdapter.TrackPages(response);

          var tracks = new List<TrackSimple>();
          await foreach (var track in spotify.Paginate(paging))
          {
            tracks.Add(new TrackSimple(
              track.Id,
              track.Name,
              track.ExternalUrls.FirstOrDefault().Value,
              track.Album.Images.LastOrDefault()?.Url,
              SelectArtists(track)
            ));
            if (tracks.Count == 100) { break; }
          }

          return Result<TrackSimple[]>.Success(tracks.ToArray());
        }
        catch (APIUnauthorizedException error)
        {
          return Result<TrackSimple[]>.Failure(
            error.Message,
            StatusCodes.Status401Unauthorized);
        }
        catch (Exception error)
        {
          return Result<TrackSimple[]>.Failure(
            error.Message,
            StatusCodes.Status500InternalServerError);
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
    }
  }
}
