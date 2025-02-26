using Application.Abstractions;
using Application.Abstractions.Gateways;
using Domain.Aggregates;
using Domain.Aggregates.Projections;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.UseCases.Queries;

public record GetPostByIdQuery(PostId PostId);

public record GetPostByIdResponse(Guid PostId, string Title, string Content, List<CommentResponse> Comments);
public record CommentResponse(Guid CommentId, string Content);

public interface IGetPostByIdInteractor : IInteractor<GetPostByIdQuery, GetPostByIdResponse?>;

public class GetPostByIdInteractor(
    IProjectionGateway<Projection.Post> postProjectionGateway,
    IProjectionGateway<Projection.Comment> commentProjectionGateway) : IGetPostByIdInteractor
{
    public async Task<GetPostByIdResponse?> InteractAsync(GetPostByIdQuery query, CancellationToken cancellationToken)
    {
        var postCollection = postProjectionGateway.GetCollection();
        var commentCollection = commentProjectionGateway.GetCollection();

        var aggregationPipeline = new[]
        {
            new BsonDocument("$match",
                new BsonDocument("_id", query.PostId.ToString())),

            new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", commentCollection.CollectionNamespace.CollectionName },
                    { "localField", "_id" },
                    { "foreignField", "PostId" },
                    { "as", "Comments" }
                }),

            new BsonDocument("$project",
                new BsonDocument
                {
                    { "_id", 1 },
                    { "Title", 1 },
                    { "Content", 1 },
                    { "Comments", 1 }
                })
        };

        var aggregatedPost = await postCollection
            .Aggregate<BsonDocument>(aggregationPipeline, cancellationToken: cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);

        if (aggregatedPost == null)
            return null;

        var comments = aggregatedPost["Comments"]
            .AsBsonArray
            .Select(comment => new CommentResponse(
                CommentId: Guid.Parse(comment["_id"].AsString),
                Content: comment["Content"].AsString
            ))
            .ToList();

        var response = new GetPostByIdResponse(
            PostId: Guid.Parse(aggregatedPost["_id"].AsString),
            Title: aggregatedPost["Title"].AsString,
            Content: aggregatedPost["Content"].AsString,
            Comments: comments
        );

        return response;
    }
}