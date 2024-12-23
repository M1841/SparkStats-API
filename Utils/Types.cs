namespace SparkStatsAPI.Utils;

public record Profile(
  string Name, string Url, string? PictureUrl)
{ }

public record TrackSimple(
  string Title, string? Url, ArtistBase[] Artists, string? PictureUrl)
{ }

public record ArtistBase(
  string Name, string Url)
{ }

public record ArtistSimple(
  string Name, string Url, string[] Genres, string? PictureUrl)
{ }

public record PlaylistSimple(
  string Id, string? Title, string? Url, int TrackCount, string? PictureUrl)
{ }
