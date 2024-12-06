using SparkStatsAPI.Extensions;

namespace SparkStatsAPI.Utils;

public static class DotEnv
{
  public static void Load(string filePath)
  {
    if (!File.Exists(filePath)) { return; }

    foreach (var line in File.ReadAllLines(filePath))
    {
      var kvPair = line.Trim().Split('=',
        options: StringSplitOptions.RemoveEmptyEntries);

      if (kvPair.Length != 2) { continue; }

      var key = kvPair[0].Trim();
      var value = kvPair[1].Trim().Unquote();

      Environment.SetEnvironmentVariable(key, value);
    }
  }
}
