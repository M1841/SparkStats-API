using SpotifyAPI.Web;

namespace SparkStatsAPI
{
  namespace Utils
  {
    public static class PagingAdapter
    {
      public static Paging<FullArtist> ArtistPages(UsersTopArtistsResponse response)
      {
        return new Paging<FullArtist>
        {
          Href = response.Href,
          Items = response.Items,
          Limit = response.Limit,
          Next = response.Next,
          Offset = response.Offset,
          Previous = response.Previous,
          Total = response.Total
        };
      }
      public static Paging<FullTrack> TrackPages(UsersTopTracksResponse response)
      {
        return new Paging<FullTrack>
        {
          Href = response.Href,
          Items = response.Items,
          Limit = response.Limit,
          Next = response.Next,
          Offset = response.Offset,
          Previous = response.Previous,
          Total = response.Total
        };
      }
    }
  }
}
