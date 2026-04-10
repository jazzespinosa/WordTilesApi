
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WordTilesApi.Data;
using WordTilesApi.env;
using WordTilesApi.Services.Implementations;
using WordTilesApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Initialize Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
  Credential = GoogleCredential.GetApplicationDefault(),
  ProjectId = "wordtiles1",
});

// Get from config 
var sqlitePath = builder.Configuration["SQLITE_DB_PATH"];
if (string.IsNullOrWhiteSpace(sqlitePath))
{
  throw new InvalidOperationException("SQLite DB path is not set.");
}
var connectionString = $"Data Source={sqlitePath};Cache=Shared;";

// Add DbContext
builder.Services.AddDbContext<GameContext>(options => options.UseSqlite(connectionString));

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWordService, WordService>();
builder.Services.AddScoped<IUtilService, UtilService>();
builder.Services.AddSingleton<MyEnvironment>();
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(
      policy =>
      {
        policy.WithOrigins(["https://wordtiles1.web.app", "https://wordtiles1.firebaseapp.com"])
                .AllowAnyHeader()
                .AllowAnyMethod();
      });
  options.AddPolicy("AllowDevelopmentOrigins",
      policy =>
      {
        policy.WithOrigins(["http://localhost:4200"])
                .AllowAnyHeader()
                .AllowAnyMethod();
      });
});
// Set up JWT Authentication for Firebase
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.Authority = "https://securetoken.google.com/wordtiles1";
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidIssuer = $"https://securetoken.google.com/wordtiles1",
        ValidateAudience = true,
        ValidAudience = "wordtiles1",
        ValidateLifetime = true
      };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Apply migrations automatically (optional)
using (var scope = app.Services.CreateScope())
{
  var gameDb = scope.ServiceProvider.GetRequiredService<GameContext>();
  gameDb.Database.Migrate();

  gameDb.Database.OpenConnection();
  gameDb.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
  gameDb.Database.CloseConnection();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();

  app.UseCors("AllowDevelopmentOrigins");
}
else
{
  app.UseCors();
}

  app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
//app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

