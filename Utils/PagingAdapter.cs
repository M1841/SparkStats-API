using SpotifyAPI.Web;

namespace SparkStatsAPI.Utils;

public static class PagingAdapter
{
  public static Paging<T> From<T>(dynamic request)
  {
    return new Paging<T>
    {
      Href = request.Href,
      Items = request.Items,
      Limit = request.Limit,
      Next = request.Next,
      Offset = request.Offset,
      Previous = request.Previous,
      Total = request.Total
    };
  }
}
