using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Domain.Common.Interfaces;
using Snowball_Legacy.Domain.Entities;
using System.Reflection;

namespace Snowball_Legacy.Infrastructure.Contexts;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Game> Game => Set<Game>();
    public DbSet<GameInfo> GameInfo => Set<GameInfo>();
    public DbSet<GameFile> GameFile => Set<GameFile>();
    public DbSet<GameImages> GameImages => Set<GameImages>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e is { Entity: IAuditableEntity, State: EntityState.Added or EntityState.Modified });
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ((IAuditableEntity)entry.Entity).CreatedDate = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    ((IAuditableEntity)entry.Entity).UpdatedDate = DateTime.UtcNow;
                    break;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
