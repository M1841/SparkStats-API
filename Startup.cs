using SparkStatsAPI.Utils;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;

namespace SparkStatsAPI
{
  public class Startup(IConfiguration configuration)
  {
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors(options =>
            {
              options.AddDefaultPolicy(policy =>
              {
                policy.WithOrigins(
                  Environment.GetEnvironmentVariable("FRONTEND_URL")!)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
              });
            });
      services.AddSingleton(
        SpotifyClientConfig
        .CreateDefault()
        .WithHTTPLogger(new SimpleConsoleHTTPLogger())
      );
      services.AddScoped<SpotifyClientBuilder>();

      services.AddControllers();
      services.AddEndpointsApiExplorer();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseHttpsRedirection();
      app.UseCors();
      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
