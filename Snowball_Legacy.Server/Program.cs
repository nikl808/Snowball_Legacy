using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
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

//Add Serilog
builder.Host.UseSerilog((context, configuration) => configuration
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(builder.Environment.ContentRootPath, "Logs", $"log-{DateTime.Today.ToShortDateString()}.txt"),
        rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapControllers();
app.UseCors("CorsPolicy");

app.MapFallbackToFile("/index.html");

app.Run();
