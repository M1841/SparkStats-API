using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;
using SparkStatsAPI.Utils;

DotEnv.Load(Path.Combine(
  Directory.GetCurrentDirectory(),
  ".env"));

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

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
app.UseAuthentication();
app.UseAuthorization();

app.Run();
