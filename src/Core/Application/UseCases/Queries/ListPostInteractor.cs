using Application.Abstractions;
using Application.Abstractions.Gateways;
using Domain.Aggregates.Projections;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.UseCases.Queries;

public record ListPostQuery();

public record ListPostResponse(Guid PostId, string Title, string Content, ulong TotalComments);

public interface IListPostInteractor : IInteractor<ListPostQuery, List<ListPostResponse>>;

public class ListPostInteractor(
    IProjectionGateway<Projection.Post> postProjectionGateway,
    IProjectionGateway<Projection.Comment> commentProjectionGateway) : IListPostInteractor
{
    public async Task<List<ListPostResponse>> InteractAsync(ListPostQuery query, CancellationToken cancellationToken)
    {
        var postCollection = postProjectionGateway.GetCollection();
        var commentCollection = commentProjectionGateway.GetCollection();

        var aggregationPipeline = new[]
        {
            new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", commentCollection.CollectionNamespace.CollectionName },
                    { "localField", "_id" },
                    { "foreignField", "PostId" },
                    { "as", "Comments" } 
                }),

            new BsonDocument("$addFields",
                new BsonDocument
                {
                    { "TotalComments", new BsonDocument("$size", "$Comments") } 
                }),

            new BsonDocument("$project",
                new BsonDocument
                {
                    { "_id", 1 }, 
                    { "Title", 1 },
                    { "Content", 1 },
                    { "TotalComments", 1 }
                })
        };

        var aggregatedPosts = await postCollection
            .Aggregate<BsonDocument>(aggregationPipeline, cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken);

        var response = aggregatedPosts.Select(post => new ListPostResponse(
            PostId: Guid.Parse(post["_id"].AsString),
            Title: post["Title"].AsString,
            Content: post["Content"].AsString,
            TotalComments: (ulong)post["TotalComments"].AsInt32
        )).ToList();

        return response;
    }
}