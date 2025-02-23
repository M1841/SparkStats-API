namespace SparkStatsAPI
{
  namespace Utils
  {
    public class Result<T>(T? ok, Error? error)
    {
      public T? Ok { get; } = ok;
      public Error? Error { get; } = error;
      public bool IsSuccess => Error == null;

      public static Result<T> Success(T result)
        => new(result, null);
      public static Result<T> Failure(string message, int status)
        => new(default,
        new Error(message, status));
    }

    public record Error(string Message, int Status);
  }
}
