namespace SparkStatsAPI.Utils;

public record Profile(
  string Name, string Url, string? Picture)
{ }

public record TrackSimple(
  string Title, string Url, ArtistBase[] Artists, string? Picture)
{ }

public record ArtistBase(
  string Name, string Url)
{ }

public record ArtistSimple(
  string Name, string Url, string[] Genres, string? Picture)
{ }
