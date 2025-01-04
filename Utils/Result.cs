namespace SparkStatsAPI.Utils;

public class Result<T>(T? ok, Error? error)
{
  public T? Ok { get; } = ok;
  public Error? Error { get; } = error;
  public bool IsSuccess => Error == null;

  public static Result<T> Success(T result)
    => new(result, null);
  public static Result<T> Failure(Error error)
    => new(default, error);
}

public record Error(string Message, int Status);
