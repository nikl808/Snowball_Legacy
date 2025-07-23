namespace Snowball_Legacy.Application.Interfaces;

public interface IGameInfoService<T> : IGenericService<T> where T : class
{
    Task<T?> GetByForeignIdAsync(int foreignId, CancellationToken token);
}
