using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Application.Interfaces.Repositories;
using Snowball_Legacy.Domain.Entities;
using Snowball_Legacy.Infrastructure.Contexts;
using System.Linq.Expressions;

namespace Snowball_Legacy.Infrastructure.Repositories;

public sealed class GameFileRepository(ApplicationDbContext context) : IGenericRepositoryExt<GameFile>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<GameFile>> GetAllAsync(CancellationToken token) => await _context.GameFile.AsNoTracking().ToListAsync(token);
    
    public async Task<GameFile?> GetByIdAsync(int id, CancellationToken token) => 
        await _context.GameFile.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);
    public async Task<GameFile?> GetByIdAsync(Expression<Func<GameFile, bool>> predicate, CancellationToken token) =>
        await _context.GameFile.AsNoTracking()
            .Include(g => g.Game)
            .FirstOrDefaultAsync(predicate, token);
    
    public async Task<GameFile?> AddAsync(GameFile entity, CancellationToken token)
    {
        await _context.GameFile.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);
        return entity;
    }
    public async Task UpdateAsync(GameFile entity, CancellationToken token)
    {
        _context.GameFile.Update(entity);
        await _context.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        var file = await _context.GameFile.FindAsync(id, token);
        if (file is not null)
        {
            _context.GameFile.Remove(file);
        }
        await _context.SaveChangesAsync(token);
    }
}