using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Snowball_Legacy.Application.DTOs;
using Snowball_Legacy.Application.Interfaces;
using Snowball_Legacy.Application.Interfaces.Repositories;
using Snowball_Legacy.Application.Services;
using Snowball_Legacy.Domain.Entities;
using Snowball_Legacy.Infrastructure.Contexts;
using Snowball_Legacy.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//New services
builder.Services.AddScoped<IGenericRepository<Game>, GameRepository>();
builder.Services.AddScoped<IGenericRepository<GameInfo>, GameInfoRepository>();
builder.Services.AddScoped<IGenericRepository<GameFile>, GameFileRepository>();
builder.Services.AddScoped<IGenericRepositoryExt<GameImages>, GameImagesRepository>();
builder.Services.AddScoped<IGenericService<GameDto>, GameService>();
builder.Services.AddScoped<IGenericService<GameInfoDto>, GameInfoService>();
builder.Services.AddScoped<IGenericFileService<GameFilesDto>, GameFileService>();
builder.Services.AddScoped<IGenericFileService<GameFilesDto>, GameImageService>();

//Database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("GameTestDataConnection"), b=>b.MigrationsAssembly("Snowball_Legacy.Server")));

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
                                .Select(sd => sd.ServiceType)
                                .ToList();

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
