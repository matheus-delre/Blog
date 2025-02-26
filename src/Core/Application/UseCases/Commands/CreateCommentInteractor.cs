using Application.Abstractions;
using Application.Services;
using Domain.Aggregates;
using Domain.ValueObjects;

namespace Application.UseCases.Commands;

public record CreateCommentCommand(PostId PostId, Content Comment);

public interface ICreateCommentInteractor : IInteractor<CreateCommentCommand>;

public class CreateCommentInteractor(IApplicationService service) : ICreateCommentInteractor
{
    public async Task InteractAsync(CreateCommentCommand cmd, CancellationToken cancellationToken)
    {
        var post = await service.LoadAggregateAsync<Post, PostId>(cmd.PostId, cancellationToken);

        post.AddComment(cmd.Comment);

        await service.AppendEventsAsync<Post, PostId>(post, cancellationToken);;
    }
}