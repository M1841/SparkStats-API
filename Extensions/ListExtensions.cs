namespace SparkStatsAPI.Extensions;

public static class ListExtensions
{
  public static List<T> Shuffle<T>(this List<T> input)
  {
    var random = new Random();
    foreach (var i in Enumerable.Range(0, input.Count - 1).OrderDescending())
    {
      var j = random.Next(i + 1);
      Console.WriteLine($"{i} {j}");
      (input[j], input[i]) = (input[i], input[j]);
    }

    return input;
  }
}
