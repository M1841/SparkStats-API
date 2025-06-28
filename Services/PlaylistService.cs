using SpotifyAPI.Web;
using SparkStatsAPI.Extensions;
using SparkStatsAPI.Utils;

namespace SparkStatsAPI
{
  namespace Services
  {
    public class PlaylistService(SpotifyClientBuilder builder)
    {
      public async Task<Result<PlaylistSimple[]>> GetAll(string authHeader)
      {
        try
        {
          var buildResult = builder.Build(authHeader);
          if (!buildResult.IsSuccess)
          {
            return Result<PlaylistSimple[]>.Failure(
              buildResult.Error!.Message,
              buildResult.Error!.Status);
          }
          var spotify = buildResult.Ok!;

          var request =
            new PlaylistCurrentUsersRequest() { Limit = 50 };
          var paging = await spotify
            .Playlists.CurrentUsers(request);

          var playlists = new List<PlaylistSimple>();
          await foreach (var playlist in spotify.Paginate(paging))
          {
            playlists.Add(new PlaylistSimple(
              playlist.Id!,
              playlist.Name!,
              playlist.ExternalUrls?.FirstOrDefault().Value!,
              playlist.Images?.LastOrDefault()?.Url,
              new UserProfileBase(
                playlist.Owner?.DisplayName ?? "",
                playlist.Owner?.ExternalUrls?.FirstOrDefault().Value ?? ""
              ),
              playlist.Tracks?.Total ?? 0
            ));
          }

          return Result<PlaylistSimple[]>.Success(playlists.ToArray());
        }
        catch (APIUnauthorizedException error)
        {
          return Result<PlaylistSimple[]>.Failure(
            error.Message,
            StatusCodes.Status401Unauthorized);
        }
        catch (Exception error)
        {
          return Result<PlaylistSimple[]>.Failure(
            error.Message,
            StatusCodes.Status500InternalServerError);
        }
      }

      public async Task<Result<PlaylistSimple>> Shuffle(
        ShuffleRequest request, string authHeader)
      {
        try
        {
          var buildResult = builder.Build(authHeader);
          if (!buildResult.IsSuccess)
          {
            return Result<PlaylistSimple>.Failure(
              buildResult.Error!.Message,
              buildResult.Error!.Status);
          }
          var spotify = buildResult.Ok!;
          var id = request.Id;

          var playlist = await spotify
            .Playlists.Get(id);
          var userId = (await spotify.UserProfile.Current()).Id;

          var createRequest = new PlaylistCreateRequest(
            (playlist.Name ?? "") + " - Shuffled")
          { Public = false, Collaborative = false, Description = "" };

          var newPlaylist = await spotify
            .Playlists.Create(userId, createRequest);

          if (newPlaylist.Id == null)
          {
            throw new Exception("Can't create new playlist");
          }
          var paging = await spotify
          .Playlists.GetItems(id);

          var tracks = new List<string>();
          await foreach (var item
            in spotify.Paginate(paging))
          {
            if (item.Track.Type == ItemType.Track)
            {
              var track = (FullTrack)item.Track;
              if (!track.IsLocal) { tracks.Add(track.Uri); }
            }
          }
          tracks = tracks.Shuffle();

          var chunks = new List<List<string>>();
          foreach (var i in Enumerable
            .Range(0, (int)Math.Ceiling(tracks.Count / 100.0)))
          {
            var start = i * 100;
            var end = Math.Min((i + 1) * 100, tracks.Count);
            chunks.Add(tracks[start..end]);
          }

          var tasks = chunks.Select(chunk =>
          {
            var request = new PlaylistAddItemsRequest(chunk);
            return spotify.Playlists
              .AddItems(newPlaylist.Id, request);
          });
          await Task.WhenAll(tasks);

          var pictureUrl = (await spotify.Playlists.Get(newPlaylist.Id)).Images?.LastOrDefault()?.Url;

          return Result<PlaylistSimple>.Success(new PlaylistSimple(
            newPlaylist.Id,
            newPlaylist.Name!,
            newPlaylist.ExternalUrls?.FirstOrDefault().Value!,
            pictureUrl,
            new UserProfileBase(
              playlist.Owner?.DisplayName ?? "",
              playlist.Owner?.ExternalUrls?.FirstOrDefault().Value ?? ""
            ),
            tracks.Count
          ));
        }
        catch (APIUnauthorizedException error)
        {
          return Result<PlaylistSimple>.Failure(
            error.Message,
            StatusCodes.Status401Unauthorized);
        }
        catch (Exception error)
        {
          return Result<PlaylistSimple>.Failure(
            error.Message,
            StatusCodes.Status500InternalServerError);
        }
      }
    }
  }
}
