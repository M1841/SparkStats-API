using SparkStatsAPI.Utils;

namespace SparkStatsAPI
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      DotEnv.Load();
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseStartup<Startup>();
        });
  }
}
