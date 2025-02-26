using Application.Abstractions.Gateways;
using Domain.Abstractions.Identities;
using Domain.Aggregates.Projections;
using Infrastructure.Projections.Abstractions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;
using System.Linq.Expressions;

namespace Infrastructure.Projections
{
    public class ProjectionGateway<TProjection>(IMongoDbContext context) : IProjectionGateway<TProjection>
    where TProjection : IProjection
    {
        private readonly IMongoCollection<TProjection> _collection = context.GetCollection<TProjection>();

        public Task<TProjection?> GetAsync<TId>(TId id, CancellationToken cancellationToken) where TId : IIdentifier
            => FindAsync(projection => projection.Id.Equals(id), cancellationToken);

        public Task<TProjection?> FindAsync(Expression<Func<TProjection, bool>> predicate, CancellationToken cancellationToken)
            => _collection.AsQueryable().Where(predicate).FirstOrDefaultAsync(cancellationToken)!;

        public Task<List<TProjection>?> ListAsync(CancellationToken cancellationToken)
            => _collection.AsQueryable()?.ToListAsync(cancellationToken)!;

        public ValueTask ReplaceInsertAsync(TProjection replacement, CancellationToken cancellationToken)
            => OnReplaceAsync(replacement, projection => projection.Id == replacement.Id, cancellationToken);

        public IMongoCollection<TProjection> GetCollection()
                => _collection;

        private async ValueTask OnReplaceAsync(TProjection replacement, Expression<Func<TProjection, bool>> filter, CancellationToken cancellationToken)
        {
            try
            {
                await _collection.ReplaceOneAsync(filter, replacement, new ReplaceOptions { IsUpsert = true }, cancellationToken);
            }
            catch (MongoWriteException e) when (e.WriteError.Category is ServerErrorCategory.DuplicateKey)
            {
                Log.Warning(
                    "By passing Duplicate Key when inserting {ProjectionType} with Id {Id}",
                    typeof(TProjection).Name, replacement.Id);
            }
        }
    }
}
