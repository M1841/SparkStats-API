using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;
using SparkStatsAPI.Utils;

DotEnv.Load(Path.Combine(
  Directory.GetCurrentDirectory(),
  ".env"));

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

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

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.UseCors();

app.Run();
