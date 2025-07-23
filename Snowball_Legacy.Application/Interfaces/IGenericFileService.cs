namespace Snowball_Legacy.Application.Interfaces;

public interface IGenericFileService<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<T?> GetByForeignIdAsync(int foreignId, CancellationToken cancellationToken);
    Task<T?> AddAsync(T entity, CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
