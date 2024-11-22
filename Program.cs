using Microsoft.AspNetCore.Authentication.Cookies;
using SparkStatsAPI.Services;
using SpotifyAPI.Web;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddHttpContextAccessor();
services.AddSingleton(SpotifyClientConfig.CreateDefault());
services.AddScoped<SpotifyClientBuilder>();

services.AddAuthorizationBuilder()
  .AddPolicy("Spotify", policy =>
  {
    policy.AuthenticationSchemes.Add("Spotify");
    policy.RequireAuthenticatedUser();
  });

services
  .AddAuthentication(options =>
  {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  })
  .AddCookie(options =>
  {
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
  })
  .AddSpotify(options =>
  {
    options.ClientId = builder.Configuration["Auth:ClientId"]!;
    options.ClientSecret = builder.Configuration["Auth:ClientSecret"]!;
    options.CallbackPath = "/auth/callback";
    options.SaveTokens = true;

    var scopes = new List<string> {
      Scopes.UserReadEmail, Scopes.UserReadPrivate
    };
    options.Scope.Add(string.Join(",", scopes));
  });

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
