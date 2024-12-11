namespace SparkStatsAPI.Utils;

public record Profile(
  string Name, string Url, string? Picture)
{ }

public record Track(
  string Title, string Url, Artist[] Artists, string? Picture)
{ }

public record Artist(
  string Name, string Url)
{ }
