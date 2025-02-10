using SparkStatsAPI.Extensions;

namespace SparkStatsAPI
{
  namespace Utils
  {
    public static class DotEnv
    {
      public static void Load()
      {
        var path = Path.Combine(
          Directory.GetCurrentDirectory(),
          ".env");

        if (!File.Exists(path)) { return; }

        foreach (var line in File.ReadAllLines(path))
        {
          var pair = line.Trim().Split('=',
            options: StringSplitOptions.RemoveEmptyEntries);

          if (pair.Length != 2) { continue; }

          var key = pair[0].Trim();
          var value = pair[1].Trim().Unquote();

          Environment.SetEnvironmentVariable(key, value);
        }
      }
    }
  }
}
