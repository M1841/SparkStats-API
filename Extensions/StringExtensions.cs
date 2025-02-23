using System.Globalization;

namespace SparkStatsAPI
{
  namespace Extensions
  {
    public static class StringExtensions
    {
      public static bool IsQuote(this string input)
      {
        if (string.IsNullOrEmpty(input)) { return false; }

        char first = input[0];
        char last = input[^1];

        return (first == '"' && last == '"')
          || (first == '\'' && last == '\'');
      }

      public static string Unquote(this string input)
      {
        return input.IsQuote() ? input[1..^1] : input;
      }

      public static string ToTitleCase(this string input)
      {
        if (_knownAcronyms.Contains(
          input, StringComparer.CurrentCultureIgnoreCase))
        {
          return _knownAcronyms.FirstOrDefault(
            str => str.Equals(
              input, StringComparison.CurrentCultureIgnoreCase))!;
        }

        TextInfo info = CultureInfo.CurrentCulture.TextInfo;
        return info.ToTitleCase(input);
      }

      private static readonly string[] _knownAcronyms = ["EDM", "EDM Trap", "IDM", "CEDM", "UK Garage"];
    }
  }
}
