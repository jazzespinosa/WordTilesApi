
using WordledDictionaryApi.Data;
using Microsoft.EntityFrameworkCore;
using WordledDictionaryApi.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<GameContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGameService, GameService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

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

app.UseCors();
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

