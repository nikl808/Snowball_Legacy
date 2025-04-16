using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Server.Models;

namespace Snowball_Legacy.Server.Contexts;

public class DataContext : DbContext
{
    public DbSet<Game> Game => Set<Game>();
    public DbSet<GameInfo> GameInfo => Set<GameInfo>();
    public DbSet<GameFile> GameFile => Set < GameFile>();
    public DbSet<GameTitlePicture> GameTitlePicture => Set<GameTitlePicture>();
    public DbSet<GameScreenshots> GameScreenshot => Set<GameScreenshots>();
    
    public DataContext(DbContextOptions<DataContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // modelBuilder.Entity<GameInfo>().HasMany(info => info.ScreenShoots).WithOne(x => x.GameInfo)
        //     .HasForeignKey(k => k.GameInfoId);       
    }
}