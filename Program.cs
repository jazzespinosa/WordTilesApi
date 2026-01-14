
using WordTilesApi.Data;
using Microsoft.EntityFrameworkCore;
using WordTilesApi.Services.Implementations;
using WordTilesApi.Services.Interfaces;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Initialize Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
  //Credential = CredentialFactory.FromFile<ServiceAccountCredential>("env/ServiceAccountKeys/wordtiles1-adc.json").ToGoogleCredential()
  Credential = GoogleCredential.GetApplicationDefault(),
  ProjectId = "wordtiles1",
});

// Add DbContext
builder.Services.AddDbContext<GameContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWordService, WordService>();
builder.Services.AddScoped<IUtilService, UtilService>();
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(
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
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

