using Snowball_Legacy.Domain.Common.Interfaces;
using System.Linq.Expressions;

namespace Snowball_Legacy.Application.Interfaces.Repositories;

public interface IGenericRepositoryExt<T> : IGenericRepository<T> where T : class, IEntity
{
    Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
}
