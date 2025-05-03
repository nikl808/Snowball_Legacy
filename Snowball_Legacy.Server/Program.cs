using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Snowball_Legacy.Server.Contexts;
using Snowball_Legacy.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<GameDataService>();

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

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

using var scope = app.Services.CreateScope();
var contextTypes = builder.Services
                                .Where(sd => sd.ServiceType.IsAssignableTo(typeof(DbContext)))
                                .Select(sd => sd.ServiceType);

foreach (var contextType in contextTypes)
{
    var dbContext = (DbContext)scope.ServiceProvider
                                    .GetRequiredService(contextType);

    if(dbContext.Database.GetPendingMigrations().Any())
        dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
//app.UseHttpsRedirection();

app.MapControllers();
app.UseCors("CorsPolicy");
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("/index.html");

app.Run();
