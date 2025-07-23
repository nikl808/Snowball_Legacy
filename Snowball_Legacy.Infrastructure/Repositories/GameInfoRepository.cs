using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Application.Interfaces.Repositories;
using Snowball_Legacy.Domain.Entities;
using Snowball_Legacy.Infrastructure.Contexts;
using System.Linq.Expressions;

namespace Snowball_Legacy.Infrastructure.Repositories;

public sealed class GameInfoRepository(ApplicationDbContext context) : IGenericRepositoryExt<GameInfo>
{
    private readonly ApplicationDbContext _context = context;
    
    public async Task<IEnumerable<GameInfo>> GetAllAsync(CancellationToken token) => await _context.GameInfo.AsNoTracking().ToListAsync(token);
    
    public async Task<GameInfo?> GetByIdAsync(int id, CancellationToken token) =>
        await _context.GameInfo.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<GameInfo?> GetByIdAsync(Expression<Func<GameInfo, bool>> predicate, CancellationToken token) =>
        await _context.GameInfo.AsNoTracking().Include(g=>g.Game).FirstOrDefaultAsync(predicate, token);

    public async Task<GameInfo?> AddAsync(GameInfo entity, CancellationToken token)
    {
        await _context.GameInfo.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);
        return entity;
    }

    public async Task UpdateAsync(GameInfo entity, CancellationToken token)
    {
        _context.GameInfo.Update(entity);
        await _context.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        var gameInfo = await _context.GameInfo.FindAsync( id, token);
        if (gameInfo is not null)
        {
            _context.GameInfo.Remove(gameInfo);
        }
        await _context.SaveChangesAsync(token);
    }
}
