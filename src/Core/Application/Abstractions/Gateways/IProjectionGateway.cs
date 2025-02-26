using Domain.Abstractions.Identities;
using Domain.Aggregates.Projections;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Application.Abstractions.Gateways;

public interface IProjectionGateway<TProjection>
    where TProjection : IProjection
{
    Task<TProjection?> FindAsync(Expression<Func<TProjection, bool>> predicate, CancellationToken cancellationToken);
    Task<TProjection?> GetAsync<TId>(TId id, CancellationToken cancellationToken) where TId : IIdentifier;
    Task<List<TProjection>?> ListAsync(CancellationToken cancellationToken);
    ValueTask ReplaceInsertAsync(TProjection replacement, CancellationToken cancellationToken);
    IMongoCollection<TProjection> GetCollection();
}