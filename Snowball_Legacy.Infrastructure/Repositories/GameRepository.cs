using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Application.Interfaces.Repositories;
using Snowball_Legacy.Domain.Entities;
using Snowball_Legacy.Infrastructure.Contexts;

namespace Snowball_Legacy.Infrastructure.Repositories;
public sealed class GameRepository(ApplicationDbContext context) : IGenericRepository<Game>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Game>> GetAllAsync(CancellationToken token) => 
        await _context.Game.AsNoTracking().ToListAsync(token);
    
    public async Task<Game?> GetByIdAsync(int id, CancellationToken token) =>
        await _context.Game.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);
    
    public async Task<Game?> AddAsync(Game entity, CancellationToken token)
    {
        await _context.Game.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);
        return entity;
    }
    public async Task UpdateAsync(Game entity, CancellationToken token)
    {
        _context.Game.Update(entity);
        await _context.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        var game = await _context.Game.FindAsync( id, token);
        if (game is not null) {
            _context.Game.Remove(game);
        }
        await _context.SaveChangesAsync(token);
    }
}