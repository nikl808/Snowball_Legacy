using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Application.Interfaces.Repositories;
using Snowball_Legacy.Domain.Entities;
using Snowball_Legacy.Infrastructure.Contexts;
using System.Linq.Expressions;

namespace Snowball_Legacy.Infrastructure.Repositories;

public sealed class GameImagesRepository(ApplicationDbContext context) : IGenericRepositoryExt<GameImages>
{
    private readonly ApplicationDbContext _context = context;
   
    public async Task<IEnumerable<GameImages>> GetAllAsync(CancellationToken token) => await _context.GameImages.AsNoTracking().ToListAsync(token);
    
    public async Task<GameImages?> GetByIdAsync(int id, CancellationToken token) =>
        await _context.GameImages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);
    
    public async Task<GameImages?> GetByIdAsync(Expression<Func<GameImages, bool>> predicate, CancellationToken token) =>
        await _context.GameImages.AsNoTracking().Include(g=>g.Game).FirstOrDefaultAsync(predicate, token);
    
    public async Task<GameImages?> AddAsync(GameImages entity, CancellationToken token)
    {
        await _context.GameImages.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);
        return entity;
    }
    public async Task UpdateAsync(GameImages entity, CancellationToken token)
    {
        _context.GameImages.Update(entity);
        await _context.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        var images = await _context.GameImages.FindAsync(id, token);
        if (images is not null)
        {
            _context.GameImages.Remove(images);
        }
        await _context.SaveChangesAsync(token);
    }
}
