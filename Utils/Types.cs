namespace SparkStatsAPI
{
  namespace Utils
  {
    public record UserProfileBase(
      string Name,
      string Url);

    public record UserProfileSimple(
      string Id,
      string Name,
      string Url,
      string? PictureUrl) : UserProfileBase(Name, Url);

    public record TrackSimple(
      string Id,
      string Name,
      string? Url,
      string? PictureUrl,
      ArtistBase[] Artists);

    public record ArtistBase(
      string Name,
      string Url);

    public record ArtistSimple(
      string Id,
      string Name,
      string Url,
      string? PictureUrl,
      string[] Genres) : ArtistBase(Name, Url);

    public record PlaylistSimple(
      string Id,
      string? Name,
      string? Url,
      string? PictureUrl,
      UserProfileBase Owner,
      int TrackCount);

    public record RefreshRequest(
      string RefreshToken);

    public record ShuffleRequest(
      string Id);
  }
}
