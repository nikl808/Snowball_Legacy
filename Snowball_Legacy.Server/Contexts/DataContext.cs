using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Server.Models;

namespace Snowball_Legacy.Server.Contexts;

public class DataContext : DbContext
{
    DbSet<Game> Game => Set<Game>();
    DbSet<GameInfo> GameInfo => Set<GameInfo>();
    DbSet<GameFile> GameFile => Set < GameFile>();
    DbSet<GamePicture> GamePicture => Set<GamePicture>();
    
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
}