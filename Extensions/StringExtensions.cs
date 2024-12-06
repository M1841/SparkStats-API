namespace SparkStatsAPI.Extensions;

public static class StringExtensions
{
  public static bool IsQuote(this string input)
  {
    if (string.IsNullOrEmpty(input)) { return false; }

    char firstChar = input[0];
    char lastChar = input[^1];

    return (firstChar == '"' && lastChar == '"')
      || (firstChar == '\'' && lastChar == '\'');
  }

  public static string Unquote(this string input)
  {
    return input.IsQuote() ? input[1..^1] : input;
  }
}
