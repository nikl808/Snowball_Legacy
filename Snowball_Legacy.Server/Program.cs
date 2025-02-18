using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Server.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//Add Database context
var dbConnectionStrings = builder.Configuration.GetConnectionString("GameDataConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(dbConnectionStrings));

//Add cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b => b
        .SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapControllers();
app.UseCors("CorsPolicy");

app.MapFallbackToFile("/index.html");

app.Run();
