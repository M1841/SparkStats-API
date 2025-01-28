namespace SparkStatsAPI.Extensions;

public static class ListExtensions
{
  public static List<T> Shuffle<T>(this List<T> input)
  {
    var random = new Random();
    for (var i = input.Count - 1; i >= 0; i--)
    {
      var j = random.Next(i + 1);
      (input[j], input[i]) = (input[i], input[j]);
    }

    return input;
  }
}
